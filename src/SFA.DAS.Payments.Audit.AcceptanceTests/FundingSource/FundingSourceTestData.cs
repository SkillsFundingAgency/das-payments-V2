using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.AcceptanceTests.FundingSource
{
    public class FundingSourceTestData
    {
        public FundingSourceType FundingSource { get; set; }
        public decimal Amount { get; set; }
        public TransactionType  TransactionType { get; set; }
        public ContractType ContractType { get; set; }
    }
}