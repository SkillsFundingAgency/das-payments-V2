using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd
{
    public class ProviderPaymentTransactionModel
    {
        public long Id { get; set; }
        public long ProviderPeriodEndSummaryId { get; set; }
        public ProviderPeriodEndSummaryModel ProviderPeriodEndSummary { get; set; }
        public TransactionTypeAmountsByContractType TransactionTypeAmounts { get; set; }
    }
}