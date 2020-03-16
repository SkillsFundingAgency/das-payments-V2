using System;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class FundingSourceModel
    {
        public long Id { get; set; }
        public Guid EventId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public Byte CompletionStatus { get; set; }
        public Decimal CompletionAmount { get; set; }
        public Decimal InstalmentAmount { get; set; }
        public int NumberOfInstalments { get; set; }
    }
}