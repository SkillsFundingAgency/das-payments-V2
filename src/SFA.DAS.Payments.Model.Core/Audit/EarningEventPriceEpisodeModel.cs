using System;

namespace SFA.DAS.Payments.Model.Core.Audit
{
    public class EarningEventPriceEpisodeModel
    {
        public long Id { get; set; }
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
        public DateTime? EffectiveTotalNegotiatedPriceStartDate { get; set; }
        public decimal? EmployerContribution { get; set; }
        public int? CompletionHoldBackExemptionCode { get; set; }
        public decimal AgreedPrice { get; set; }
        public DateTime CourseStartDate { get; set; }

    }
}