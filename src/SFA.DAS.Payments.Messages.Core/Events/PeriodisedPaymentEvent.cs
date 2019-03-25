using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public abstract class PeriodisedPaymentEvent : PaymentsEvent, IPeriodisedPaymentEvent
    {
        public string PriceEpisodeIdentifier { get; set; }
        public decimal AmountDue { get; set; }
        public byte DeliveryPeriod { get; set; }
        public ContractType ContractType { get; set; }
    }
}