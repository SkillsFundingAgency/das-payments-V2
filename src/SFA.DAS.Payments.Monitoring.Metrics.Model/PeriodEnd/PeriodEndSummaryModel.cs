﻿namespace SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd
{
    public class PeriodEndSummaryModel
    {
        public long Id { get; set; }
        public long Ukprn { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public long JobId { get; set; }
        public decimal Percentage { get; set; }
        public ContractTypeAmountsVerbose PaymentMetrics { get; set; }
        public ContractTypeAmounts DcEarnings { get; set; }
        public ContractTypeAmounts Payments { get; set; }
        public decimal DataLockedEarnings { get; set; }
        public decimal AlreadyPaidDataLockedEarnings { get; set; }
        public decimal TotalDataLockedEarnings { get; set; }
        public ContractTypeAmounts HeldBackCompletionPayments { get; set; }
        public ContractTypeAmounts YearToDatePayments { get; set; }
    }
}