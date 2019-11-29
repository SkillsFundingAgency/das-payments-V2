namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class CurrentPriceEpisode
    {
        public long Id { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public decimal AgreedPrice { get; set; }
        public long Uln { get; set; }
        public long Ukprn { get; set; }
        public long JobId { get; set; }

        public string MessageType { get; set; }
        public string Message { get; set; }
    }
}
