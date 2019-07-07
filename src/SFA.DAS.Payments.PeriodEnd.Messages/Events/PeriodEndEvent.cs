using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.PeriodEnd.Messages.Events
{
    public abstract class PeriodEndEvent: IPeriodEndEvent
    {
        public long JobId { get; set; }
        public CollectionPeriod CollectionPeriod { get; set; }

    }
}