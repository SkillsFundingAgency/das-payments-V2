using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Monitoring.Metrics.Model.Submission
{
    public class RequiredPaymentsModel
    {
        public long Id { get; set; }
        public long SubmissionSummaryId { get; set; }
        public virtual SubmissionSummaryModel SubmissionSummary{ get; set; }
        public TransactionTypeAmounts Amounts { get; set; }
    }
}