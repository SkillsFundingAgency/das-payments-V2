using System;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data
{
    public class HistoricalPayment
    {
        public string LearnerId { get; set; }

        public string LearnRefNumber { get; set; }

        public long Ukprn { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

        public byte DeliveryPeriod { get; set; }

        public long Uln { get; set; }

        public string TransactionType { get; set; }

        public DateTimeOffset EventTime { get; set; } = DateTimeOffset.Now;
        public DateTime IlrSubmissionDateTime { get; set; } = DateTime.Now;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? PlannedEndDate { get; set; } = DateTime.Now;
        public DateTime? ActualEndDate { get; set; } = DateTime.Now;
        public DateTime? LearningStartDate { get; set; } = DateTime.Now;

        public decimal Amount { get; set; }
        public OnProgrammeEarningType Type => TransactionType.Contains("Learning") ?
            OnProgrammeEarningType.Learning :
            TransactionType.Contains("Completion") ?
                OnProgrammeEarningType.Completion :
                OnProgrammeEarningType.Balancing;
    }
}