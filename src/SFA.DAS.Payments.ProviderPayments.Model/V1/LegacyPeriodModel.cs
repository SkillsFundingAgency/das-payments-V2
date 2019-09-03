using System;

namespace SFA.DAS.Payments.ProviderPayments.Model.V1
{
    public class LegacyPeriodModel
    {
        public string PeriodName { get; set; }
        public int CalendarMonth { get; set; }
        public int CalendarYear { get; set; }
        public DateTime? AccountDataValidAt { get; set; }
        public DateTime? CommitmentDataValidAt { get; set; }
        public DateTime CompletionDateTime { get; set; }
    }
}
