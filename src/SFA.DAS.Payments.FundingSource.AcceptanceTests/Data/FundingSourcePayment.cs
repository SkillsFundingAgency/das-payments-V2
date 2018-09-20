namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Data
{
    public class FundingSourcePayment
    {
        public string LearnRefNumber { get; set; }

        public long Ukprn { get; set; }

        public long Uln { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

        public short TransactionType { get; set; }

        public short FundingSource { get; set; }

        public decimal Amount { get; set; }
    }
}