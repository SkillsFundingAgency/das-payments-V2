using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Handlers
{
    public class SfaCoInvestedFundingSourceHandler:IHandleMessages<SfaCoInvestedFundingSourcePaymentEvent>
    {
        public static List<SfaCoInvestedFundingSourcePaymentEvent>  ReceivedEvents { get; } = new List<SfaCoInvestedFundingSourcePaymentEvent>();

        public async Task Handle(SfaCoInvestedFundingSourcePaymentEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine(message.ToJson());
            ReceivedEvents.Add(message);
        }
    }
}