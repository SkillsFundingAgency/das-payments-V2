using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd
{
    public class ProviderPaymentTransactionModel
    {
        public long Id { get; set; }
        public long Ukprn { get; set; }
        public long ProviderPeriodEndSummaryId { get; set; }
        public ProviderPeriodEndSummaryModel SubmissionSummary { get; set; }

        public ContractType ContractType { get; set; }
        public TransactionTypeAmounts TransactionTypeAmounts { get; set; }
    }
}