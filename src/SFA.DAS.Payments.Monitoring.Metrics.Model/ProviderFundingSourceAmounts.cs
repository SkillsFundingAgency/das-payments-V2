using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Monitoring.Metrics.Model
{
    public class ProviderFundingSourceAmounts
    {
        public long Ukprn { get; set; }
        public ContractType ContractType { get; set; }
        public decimal FundingSource1 { get; set; }
        public decimal FundingSource2 { get; set; }
        public decimal FundingSource3 { get; set; }
        public decimal FundingSource4 { get; set; }
        public decimal FundingSource5 { get; set; }
        public decimal Total => FundingSource1 + FundingSource2 + FundingSource3 + FundingSource4 + FundingSource5;

    }
}