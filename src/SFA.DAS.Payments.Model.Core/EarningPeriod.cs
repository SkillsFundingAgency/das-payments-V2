namespace SFA.DAS.Payments.Model.Core
{
    public class EarningPeriod
    {
        public string PriceEpisodeIdentifier { get; set; }
        public CalendarPeriod Period { get; set; }
        public decimal Amount { get; set; }
    }
}