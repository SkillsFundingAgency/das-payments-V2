using System;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class EarningsModel
    {
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int CompletionStatus { get; set; }
        public decimal CompletionAmount { get; set; }
        public decimal InstalmentAmount { get; set; }
        public int NumberOfInstalments { get; set; }
    }
}