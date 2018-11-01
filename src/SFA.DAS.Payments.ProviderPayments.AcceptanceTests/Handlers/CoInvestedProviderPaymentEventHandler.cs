using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.ProviderPayments.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Handlers
{
    public class CoInvestedProviderPaymentEventHandler : IHandleMessages<CoInvestedProviderPaymentEvent>
    {
        public static List<CoInvestedProviderPaymentEvent> ReceivedEvents { get; } = new List<CoInvestedProviderPaymentEvent>();

        public async Task Handle(CoInvestedProviderPaymentEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine(message.ToJson());
            ReceivedEvents.Add(message);
            await Task.CompletedTask;
        }

    }

}