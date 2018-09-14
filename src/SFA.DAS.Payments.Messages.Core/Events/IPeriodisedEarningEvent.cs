using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public interface IPeriodisedEarningEvent: IPaymentsEvent
    {
        string PriceEpisodeIdentifier { get; }
        decimal AmountDue { get; }
        CalendarPeriod CollectionPeriod { get; }
        CalendarPeriod DeliveryPeriod { get; }
    }
}