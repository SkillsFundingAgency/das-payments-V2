using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Data
{
    public class FundingSourcePayment
    {
        public FundingSourceType FundingSourceType => FundingSource.Contains("Co-Invested Sfa") ?
               FundingSourceType.CoInvestedSfa : FundingSourceType.CoInvestedEmployer;

        public OnProgrammeEarningType Type => TransactionType.Contains("Learning") ?
               OnProgrammeEarningType.Learning : TransactionType.Contains("Completion") ?
               OnProgrammeEarningType.Completion : OnProgrammeEarningType.Balancing;

        public string TransactionType { get; set; }
        public string FundingSource { get; set; }
        public byte DeliveryPeriod { get; set; }
        public decimal Amount { get; set; }
    }
}