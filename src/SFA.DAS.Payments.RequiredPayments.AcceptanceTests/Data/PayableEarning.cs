namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data
{
    public class PayableEarning
    {
        public string LearnRefNumber { get; set; }

        public long Ukprn { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

        public short Period { get; set; }

        public long Uln { get; set; }

        public string TransactionType { get; set; }

        public decimal Amount { get; set; }
    }
}