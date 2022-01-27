using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Monitoring.Metrics.Model
{
    public class ProviderLearnerNegativeEarningsTotal
    {
        public long Ukprn { get; set; }
        public long Uln { get; set; }
        public ContractType ContractType { get; set; }
        public decimal NegativeEarningsTotal { get; set; }
    }
}