using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Metrics.Model.Submission
{
    public class SubmissionSummaryModel
    {
        public long Id { get; set; }
        public long Ukprn { get; set; }
        public long JobId { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public decimal Percentage { get; set; }
        public ContractTypeAmounts YearToDatePayments { get; set; }
        public ContractTypeAmountsVerbose SubmissionMetrics { get; set; }
        public virtual List<EarningsModel> EarningsMetrics { get; set; }
        public virtual DataLockCountsModel DataLockMetrics { get; set; } = new DataLockCountsModel();
        public virtual List<RequiredPaymentsModel> RequiredPaymentsMetrics { get; set; }
        public ContractTypeAmounts DcEarnings { get; set; }
        public ContractTypeAmountsVerbose DasEarnings { get; set; }
        public ContractTypeAmounts RequiredPayments { get; set; }
        public decimal AdjustedDataLockedEarnings { get; set; }
        public decimal TotalDataLockedEarnings { get; set; }
        public decimal AlreadyPaidDataLockedEarnings { get; set; }
        public ContractTypeAmounts HeldBackCompletionPayments { get; set; }
    }
}