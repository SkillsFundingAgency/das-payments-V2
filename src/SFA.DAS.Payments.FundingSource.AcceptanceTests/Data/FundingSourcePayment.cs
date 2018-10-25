using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Data
{
    public class FundingSourcePayment
    {
        public string PriceEpisodeIdentifier { get; set; }

        public FundingSourceType FundingSourceType => FundingSource.Contains("CoInvestedSfa") ? FundingSourceType.CoInvestedSfa : FundingSourceType.CoInvestedEmployer;

        public decimal Amount { get; set; }

        public OnProgrammeEarningType Type => TransactionType.Contains("Learning") ?
            OnProgrammeEarningType.Learning : TransactionType.Contains("Completion") ?
            OnProgrammeEarningType.Completion : OnProgrammeEarningType.Balancing;

        public string TransactionType { get; set; }

        public string FundingSource { get; set; }

        public byte DeliveryPeriod { get; set; }
    }
}