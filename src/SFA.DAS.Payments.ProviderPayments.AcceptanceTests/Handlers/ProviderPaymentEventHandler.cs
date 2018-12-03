using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.ProviderPayments.Messages;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Handlers
{
    public class ProviderPaymentEventHandler : IHandleMessages<ProviderPaymentEvent>
    {
        public static ConcurrentBag<ProviderPaymentEvent> ReceivedEvents { get; } = new ConcurrentBag<ProviderPaymentEvent>();

        public async Task Handle(ProviderPaymentEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine(message.ToJson());
            ReceivedEvents.Add(message);
            await Task.CompletedTask;
        }
    }
}