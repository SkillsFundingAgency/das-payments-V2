using System;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ProviderPayments.Messages
{
    public class RecordedAct1CompletionPaymentEvent : ProviderPaymentEvent
    {
        public FundingSourceType FundingSource { get; set; }
        
        public EarningDetails EarningDetails { get; set; }
    }

    public class EarningDetails
    {
        public DateTime StartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public byte? CompletionStatus { get; set; }
        public decimal? CompletionAmount { get; set; }
        public decimal? InstalmentAmount { get; set; }
        public short? NumberOfInstalments { get; set; }
    }
}