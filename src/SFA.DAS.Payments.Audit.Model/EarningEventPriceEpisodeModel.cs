using System;

namespace SFA.DAS.Payments.Audit.Model
{
    public class EarningEventPriceEpisodeModel
    {
        public Guid EarningEventId { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public decimal SfaContributionPercentage { get; set; }
        public decimal TotalNegotiatedPrice1 { get; set; }
        public decimal TotalNegotiatedPrice2 { get; set; }
        public decimal TotalNegotiatedPrice3 { get; set; }
        public decimal TotalNegotiatedPrice4 { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int NumberOfInstalments { get; set; }
        public decimal InstalmentAmount { get; set; }
        public decimal CompletionAmount { get; set; }
        public bool Completed { get; set; }
    }
}