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
        public DateTime PlannedEndDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public Byte CompletionStatus { get; set; }
        public Decimal CompletionAmount { get; set; }
        public Decimal InstalmentAmount { get; set; }
        public int NumberOfInstalments { get; set; }
    }
}