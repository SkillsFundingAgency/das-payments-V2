using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.PeriodEnd.Messages.Events
{
    public interface IPeriodEndEvent
    {
        long JobId { get; }
        CollectionPeriod CollectionPeriod { get; }
    }
}