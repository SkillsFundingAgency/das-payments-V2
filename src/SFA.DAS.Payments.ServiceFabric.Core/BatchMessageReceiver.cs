using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public class BatchMessageReceiver
    {
        private readonly MessageReceiver messageReceiver;
        private readonly List<Message> messages;
        public BatchMessageReceiver(ServiceBusConnection connection, string endpointName)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (endpointName == null) throw new ArgumentNullException(nameof(endpointName));
            messageReceiver = new MessageReceiver(connection, endpointName, ReceiveMode.PeekLock,
                RetryPolicy.Default, 0);
            messages = new List<Message>();
        }

        public async Task<ReadOnlyCollection<Message>> ReceiveMessages(int batchSize, CancellationToken cancellationToken)
        {
            messages.Clear();
            for (var i = 0; i < 10 && messages.Count <= batchSize; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var receivedMessages = await messageReceiver.ReceiveAsync(batchSize, TimeSpan.FromSeconds(2))
                    .ConfigureAwait(false);
                if (receivedMessages == null || !receivedMessages.Any())
                    break;
                messages.AddRange(receivedMessages);
            }
            return messages.AsReadOnly();
        }

        public async Task Complete()
        {
            await messageReceiver.CompleteAsync(messages.Select(message => message.SystemProperties.LockToken))
                .ConfigureAwait(false);
        }

        public async Task Complete(IEnumerable<string> lockTokens)
        {
            await messageReceiver.CompleteAsync(lockTokens).ConfigureAwait(false);
        }

        public async Task Complete(Message message)
        {
            await messageReceiver.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
        }

        public async Task Abandon()
        {
            await Task.WhenAll(messages.Select(message => messageReceiver.AbandonAsync(message.SystemProperties.LockToken)))
                .ConfigureAwait(false);
        }

        public async Task Abandon(IList<string> lockTokens)
        {
            await Task.WhenAll(lockTokens.Select(token => messageReceiver.AbandonAsync(token)))
                .ConfigureAwait(false);
        }

        public async Task DeadLetter(Message message)
        {
            var failedMessage =
                messages.FirstOrDefault(msg => msg.SystemProperties.LockToken == message.SystemProperties.LockToken);
            if (failedMessage == null)
                throw new InvalidOperationException($"Cannot move the message to the dead letter queue Message not found in list of received messages");
            await messageReceiver.DeadLetterAsync(failedMessage.SystemProperties.LockToken).ConfigureAwait(false);
        }

        public async Task Close()
        {
            if (!messageReceiver.IsClosedOrClosing)
                await messageReceiver.CloseAsync().ConfigureAwait(false);
        }
    }
}