using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public interface IPeriodisedPaymentEvent : IPaymentsEvent
    {
        string PriceEpisodeIdentifier { get; }
        decimal AmountDue { get; }
        byte DeliveryPeriod { get; }
        long? AccountId { get; }
        ContractType ContractType { get; }
    }
}