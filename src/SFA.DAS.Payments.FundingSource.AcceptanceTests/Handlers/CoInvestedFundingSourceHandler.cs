using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Handlers
{
    public class CoInvestedFundingSourceHandler : IHandleMessages<CoInvestedFundingSourcePaymentEvent>
    {
        public static ConcurrentBag<CoInvestedFundingSourcePaymentEvent> ReceivedEvents { get; } = new ConcurrentBag<CoInvestedFundingSourcePaymentEvent>();

        public async Task Handle(CoInvestedFundingSourcePaymentEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine(message.ToJson());
            ReceivedEvents.Add(message);
            await Task.FromResult(0);
        }

    }

}