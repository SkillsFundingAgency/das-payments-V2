namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class CurrentPriceEpisode
    {
        public string PriceEpisodeIdentifier { get; set; }
        public decimal AgreedPrice { get; set; }
        public long Uln { get; set; }
        public long Ukprn { get; set; }
        public long JobId { get; set; }
    }
}
