using System;

namespace SFA.DAS.Payments.Model.Core
{
    public class PriceEpisode
    {
        public string Identifier { get; set; }
        /// <summary>
        /// Training price
        /// </summary>
        public decimal TotalNegotiatedPrice1 { get; set; }
        /// <summary>
        /// Assessment price
        /// </summary>
        public decimal? TotalNegotiatedPrice2 { get; set; }
        /// <summary>
        /// Residual training price
        /// </summary>
        public decimal? TotalNegotiatedPrice3 { get; set; }
        /// <summary>
        /// Residual assessment price
        /// </summary>
        public decimal? TotalNegotiatedPrice4 { get; set; }
        public decimal AgreedPrice { get; set; }
        public DateTime CourseStartDate { get; set; }
        public DateTime EffectiveTotalNegotiatedPriceStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int NumberOfInstalments { get; set; }
        public decimal InstalmentAmount { get; set; }
        public decimal CompletionAmount { get; set; }
        public bool Completed { get; set; }
        public decimal? EmployerContribution { get; set; }
        public int? CompletionHoldBackExemptionCode { get; set; }
    }
}