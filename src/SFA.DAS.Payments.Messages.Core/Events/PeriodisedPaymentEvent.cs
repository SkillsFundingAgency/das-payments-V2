using System;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public abstract class PeriodisedPaymentEvent : PaymentsEvent, IPeriodisedPaymentEvent
    {
        public string PriceEpisodeIdentifier { get; set; }
        public decimal AmountDue { get; set; }
        public byte DeliveryPeriod { get; set; }
        public long? AccountId { get; set; }
        public ContractType ContractType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public byte CompletionStatus { get; set; }
        public decimal CompletionAmount { get; set; }
        public decimal InstalmentAmount { get; set; }
        public short NumberOfInstalments { get; set; }
    }
}