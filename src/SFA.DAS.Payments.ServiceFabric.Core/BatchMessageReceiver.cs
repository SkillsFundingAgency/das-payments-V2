using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

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

        //public BatchMessageReceiver(ServiceBusConnection connection, string endpointName)
        //{
        //    if (connection == null) throw new ArgumentNullException(nameof(connection));
        //    if (endpointName == null) throw new ArgumentNullException(nameof(endpointName));
        //    var r = new Azure.Messaging.ServiceBus.ServiceBusReceiver();
        //    messageReceiver = new MessageReceiver(connection, endpointName, ReceiveMode.PeekLock,
        //        RetryPolicy.Default, 0);
        //    messages = new List<Message>();
        //}

        public async Task<ReadOnlyCollection<ServiceBusReceivedMessage>> ReceiveMessages(int batchSize, CancellationToken cancellationToken)
        {
            messages.Clear();
            for (var i = 0; i < 10 && messages.Count <= batchSize; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var receivedMessages = await messageReceiver.ReceiveMessagesAsync(batchSize, TimeSpan.FromMilliseconds(500),cancellationToken)
                    .ConfigureAwait(false);
                if (receivedMessages == null || !receivedMessages.Any())
                    break;
                messages.AddRange(receivedMessages);
            }
            return messages.AsReadOnly();
        }

        //public async Task Complete(IEnumerable<string> lockTokens)
        //{
        //    //await messageReceiver.CompleteAsync(lockTokens).ConfigureAwait(false);
        //}

        //public async Task Complete(string lockToken)
        //{
        //    //await messageReceiver.CompleteAsync(lockToken);
        //}

        public async Task Complete(ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            await messageReceiver.CompleteMessageAsync(message, cancellationToken).ConfigureAwait(false);
        }
        
        public async Task Abandon(ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            await messageReceiver.AbandonMessageAsync(message, null, cancellationToken).ConfigureAwait(false);
        }

        //public async Task Abandon(IList<string> lockTokens)
        //{
        //    //await Task.WhenAll(lockTokens.Select(token => messageReceiver.AbandonAsync(token)))
        //    //    .ConfigureAwait(false);
        //}

        //public async Task Abandon(string lockToken)
        //{
        //    //await messageReceiver.AbandonAsync(lockToken);
        //}

        public async Task DeadLetter(string lockToken, CancellationToken cancellationToken)
        {
            var failedMessage = messages.FirstOrDefault(msg => msg.LockToken == lockToken)
                                ?? throw new InvalidOperationException($"Cannot move the message to the dead letter queue, Message not found in list of received messages");
            await DeadLetter(failedMessage, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeadLetter(ServiceBusReceivedMessage failedMessage, CancellationToken cancellationToken)
        {
            await messageReceiver.DeadLetterMessageAsync(failedMessage, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task Close()
        {
            if (!messageReceiver.IsClosed)
                await messageReceiver.CloseAsync().ConfigureAwait(false);
        }
    }
}