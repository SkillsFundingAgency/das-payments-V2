using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Monitoring.Metrics.Model.Submission
{
    public class EarningsModel
    {
        public long Id { get; set; }
        public SubmissionSummaryModel SubmissionSummary { get; set; }
        public EarningsType EarningsType { get; set; }
        public TransactionTypeAmounts Amounts { get; set; }
    }
}