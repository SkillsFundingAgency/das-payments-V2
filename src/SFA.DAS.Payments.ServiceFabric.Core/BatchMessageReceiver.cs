using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using Azure.Messaging.ServiceBus;
using NServiceBus.Faults;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public class BatchMessageReceiver
    {
        private readonly ServiceBusReceiver messageReceiver;
        private readonly List<ServiceBusReceivedMessage> messages;
        public BatchMessageReceiver(ServiceBusClient client, string endpointName)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (endpointName == null) throw new ArgumentNullException(nameof(endpointName));
            messageReceiver = client.CreateReceiver(endpointName);
            messages = new List<ServiceBusReceivedMessage>();
        }

        public async Task<ReadOnlyCollection<ServiceBusReceivedMessage>> ReceiveMessages(int batchSize, CancellationToken cancellationToken)
        {
            messages.Clear();
            for (var i = 0; i < 10 && messages.Count <= batchSize; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var receivedMessages = await messageReceiver.ReceiveMessagesAsync(batchSize, TimeSpan.FromMilliseconds(500), cancellationToken)
                    .ConfigureAwait(false);
                if (receivedMessages == null || !receivedMessages.Any())
                    break;
                messages.AddRange(receivedMessages);
            }
            return messages.AsReadOnly();
        }



        public async Task Complete(IEnumerable<string> lockTokens)
        {
            foreach (var serviceBusReceivedMessage in messages.Where(message => lockTokens.Any(lockToken => lockToken.Equals(message.LockToken))))
            {
                await messageReceiver.CompleteMessageAsync(serviceBusReceivedMessage).ConfigureAwait(false);
            }
        }


        public async Task Abandon(IList<string> lockTokens)
        {
            foreach (var serviceBusReceivedMessage in messages.Where(message => lockTokens.Any(lockToken => lockToken.Equals(message.LockToken))))
            {
                await messageReceiver.AbandonMessageAsync(serviceBusReceivedMessage).ConfigureAwait(false);
            }
        }

        public async Task DeadLetter(string lockToken, CancellationToken cancellationToken)
        {
            var receivedMessage = messages.FirstOrDefault(message => message.LockToken.Equals(lockToken));
            if (receivedMessage == null)
                return;
            await messageReceiver.DeadLetterMessageAsync(receivedMessage, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeadLetter(ServiceBusReceivedMessage failedMessage, CancellationToken cancellationToken)
        {
            if (failedMessage == null) throw new ArgumentNullException(nameof(failedMessage));
            await messageReceiver.DeadLetterMessageAsync(failedMessage, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task Abandon(ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            await messageReceiver.DeadLetterMessageAsync(message, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task Close()
        {
            if (!messageReceiver.IsClosed)
                await messageReceiver.CloseAsync().ConfigureAwait(false);
        }
    }
}