namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Data
{
    public class RequiredPayment
    {
        public string PriceEpisodeIdentifier { get; set; }

        public byte Period { get; set; }

        public long Uln { get; set; }

        public short TransactionType { get; set; }

        public decimal Amount { get; set; }
    }
}
