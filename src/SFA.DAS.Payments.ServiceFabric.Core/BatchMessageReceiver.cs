using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Azure.Messaging.ServiceBus;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public class BatchMessageReceiver
    {
        private readonly MessageReceiver messageReceiver;
        private readonly List<ServiceBusReceivedMessage> messages;
        public BatchMessageReceiver(ServiceBusConnection connection, string endpointName)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (endpointName == null) throw new ArgumentNullException(nameof(endpointName));
            var r = new Azure.Messaging.ServiceBus.ServiceBusReceiver()
            messageReceiver = new MessageReceiver(connection, endpointName, ReceiveMode.PeekLock,
                RetryPolicy.Default, 0);
            messages = new List<Message>();
        }

        public async Task<ReadOnlyCollection<ServiceBusReceivedMessage>> ReceiveMessages(int batchSize, CancellationToken cancellationToken)
        {
            messages.Clear();
            for (var i = 0; i < 10 && messages.Count <= batchSize; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var receivedMessages = await messageReceiver.ReceiveAsync(batchSize, TimeSpan.FromMilliseconds(500))
                    .ConfigureAwait(false);
                if (receivedMessages == null || !receivedMessages.Any())
                    break;
                messages.AddRange(receivedMessages);
            }
            return messages.AsReadOnly();
        }

        public async Task Complete(IEnumerable<string> lockTokens)
        {
            await messageReceiver.CompleteAsync(lockTokens).ConfigureAwait(false);
        }

        public async Task Complete(string lockToken)
        {
            await messageReceiver.CompleteAsync(lockToken);
        }

        public async Task Abandon(IList<string> lockTokens)
        {
            await Task.WhenAll(lockTokens.Select(token => messageReceiver.AbandonAsync(token)))
                .ConfigureAwait(false);
        }

        public async Task Abandon(string lockToken)
        {
            await messageReceiver.AbandonAsync(lockToken);
        }

        public async Task DeadLetter(ServiceBusReceivedMessage message)
        {
            var failedMessage =
                messages.FirstOrDefault(msg => msg.LockToken == message.LockToken);
            if (failedMessage == null)
                throw new InvalidOperationException($"Cannot move the message to the dead letter queue Message not found in list of received messages");
            await messageReceiver.DeadLetterAsync(failedMessage.LockToken).ConfigureAwait(false);
        }

        public async Task Close()
        {
            if (!messageReceiver.IsClosedOrClosing)
                await messageReceiver.CloseAsync().ConfigureAwait(false);
        }
    }
}