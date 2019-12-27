using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Metrics.Model.Submission
{
    public class SubmissionModel
    {
        public long Id { get; set; }
        public long Ukprn { get; set; }
        public long JobId { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public List<ProviderEarningsMetrics> EarningsMetrics { get; set; }
        public List<DataLockMetrics> DataLockedPaymentsMetrics { get; set; }
        public List<RequiredPaymentsModel> RequiredPaymentsMetrics { get; set; }
        public ContractTypeMetrics DcEarnings { get; set; }
        public ContractTypeMetrics DasEarnings { get; set; }
        public ContractTypeMetrics RequiredPayments { get; set; }
        public decimal DataLockedEarnings { get; set; }
        public decimal NonLevyRequiredPayments { get; set; }
        public ContractTypeMetrics HeldBackCompletionPayments { get; set; }
    }
}