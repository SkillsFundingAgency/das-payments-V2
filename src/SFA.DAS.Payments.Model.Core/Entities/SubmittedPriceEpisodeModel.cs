namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class SubmittedPriceEpisodeModel
    {
        public long Id { get; set; }
        public long Ukprn { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public string IlrDetails { get; set; }
    }
}
