using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    public class ProviderPaymentEventHandler : IHandleMessages<ProviderPaymentEvent>
    {
        public static ConcurrentBag<ProviderPaymentEvent> ReceivedEvents { get; } = new ConcurrentBag<ProviderPaymentEvent>();

        public Task Handle(ProviderPaymentEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received event type: {message.GetType().Name}, payload: {message.ToJson()}");
            ReceivedEvents.Add(message);
            return Task.FromResult(0);
        }
    }
}