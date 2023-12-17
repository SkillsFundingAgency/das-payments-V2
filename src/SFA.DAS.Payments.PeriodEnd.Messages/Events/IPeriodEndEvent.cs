using System;
using SFA.DAS.Payments.Messages.Common;
using SFA.DAS.Payments.Messages.Common.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.PeriodEnd.Messages.Events
{
    public interface IPeriodEndEvent: IPaymentsMessage, IEvent
    {
        DateTimeOffset EventTime { get; }
        Guid EventId { get; }
        CollectionPeriod CollectionPeriod { get; }
    }
}