using System;

namespace SFA.DAS.Payments.Model.Core
{
    public class PriceEpisode
    {
        public string Identifier { get; set; }
        public decimal TotalNegotiatedPrice1 { get; set; }
        public decimal? TotalNegotiatedPrice2 { get; set; }
        public decimal? TotalNegotiatedPrice3 { get; set; }
        public decimal? TotalNegotiatedPrice4 { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public int NumberOfInstalments { get; set; }
        public decimal InstalmentAmount { get; set; }
        public decimal CompletionAmount { get; set; }
        public bool Completed { get; set; }
    }
}