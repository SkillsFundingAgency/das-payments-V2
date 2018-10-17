using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data
{
    public class HistoricalPayment
    {
        public string LearnerId { get; set; }

        public string LearnRefNumber { get; set; }

        public long Ukprn { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

        public byte Delivery_Period { get; set; }

        public long Uln { get; set; }

        public string TransactionType { get; set; }

        public decimal Amount { get; set; }
        public OnProgrammeEarningType Type => TransactionType.Contains("Learning") ? 
            OnProgrammeEarningType.Learning :
            TransactionType.Contains("Completion") ? 
                OnProgrammeEarningType.Completion :
                OnProgrammeEarningType.Balancing;
    }
}