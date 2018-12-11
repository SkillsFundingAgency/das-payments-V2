using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Handlers
{
    public class FundingSourcePaymentEventHandler : IHandleMessages<FundingSourcePaymentEvent>
    {
        public static ConcurrentBag<FundingSourcePaymentEvent> ReceivedEvents { get; } = new ConcurrentBag<FundingSourcePaymentEvent>();

        public async Task Handle(FundingSourcePaymentEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine(message.ToJson());
            ReceivedEvents.Add(message);
            await Task.FromResult(0);
        }
    }
}