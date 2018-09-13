namespace SFA.DAS.Payments.Model.Core
{
    public class EarningPeriod
    {
        public string PriceEpisodeIdentifier { get; set; }
        public byte Period { get; set; }
        public decimal Amount { get; set; }
    }
}