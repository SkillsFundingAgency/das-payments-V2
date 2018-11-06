using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public abstract class Payment
    {
        public OnProgrammeEarningType Type => TransactionType.Contains("Learning") ?
            OnProgrammeEarningType.Learning : TransactionType.Contains("Completion") ?
                OnProgrammeEarningType.Completion : OnProgrammeEarningType.Balancing;
        public FundingSourceType FundingSourceType => FundingSource.Contains("CoInvestedSfa") ?
            FundingSourceType.CoInvestedSfa : FundingSourceType.CoInvestedEmployer;

        public string TransactionType { get; set; }
        public string FundingSource { get; set; }
        public byte DeliveryPeriod { get; set; }
        public decimal Amount { get; set; }
    }
}
