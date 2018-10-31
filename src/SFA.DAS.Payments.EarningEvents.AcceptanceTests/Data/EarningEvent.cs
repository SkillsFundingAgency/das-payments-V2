namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Data
{
    public class EarningEvent
    {
        public string PriceEpisodeIdentifier { get; set; }

        public short Period { get; set; }

        public string TransactionType { get; set; }

        public decimal Amount { get; set; }
    }
}
