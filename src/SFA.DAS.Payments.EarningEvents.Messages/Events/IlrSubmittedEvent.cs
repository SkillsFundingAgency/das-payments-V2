using System;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class IlrSubmittedEvent
    {
        public string JobId { get; set; }

        public DateTime SubmissionDate { get; set; }

        public long Ukprn { get; set; }
    }
}
