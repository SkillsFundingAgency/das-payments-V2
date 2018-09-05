using System;

namespace SFA.DAS.Payments.FundingSource.Messages.Events
{
    public class FundingSourcePaymentEvent : IFundingSourcePaymentEvent
    {
        public DateTimeOffset EventTime { set; get; }

        public string JobId { set; get; }

        public decimal Amount { get; set; }
    }
}