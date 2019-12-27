using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Monitoring.Metrics.Model.Submission
{
    public class RequiredPaymentsModel
    {
        public long Id { get; set; }
        public SubmissionModel SubmissionMOdel { get; set; }
        public ContractType ContractType { get; set; }
        public TransactionTypeCounts Counts { get; set; }
    }
}