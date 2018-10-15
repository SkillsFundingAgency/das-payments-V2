using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Data
{
    public class RequiredPayment
    {
        public string PriceEpisodeIdentifier { get; set; }

        public byte Period { get; set; }

        public byte DeliveryPeriod { get; set; }

        public OnProgrammeEarningType Type => TransactionType.Contains("Learning") ?
                OnProgrammeEarningType.Learning : TransactionType.Contains("Completion") ?
                OnProgrammeEarningType.Completion : OnProgrammeEarningType.Balancing;

        public string TransactionType { get; set; }

        public decimal Amount { get; set; }
    }
}
