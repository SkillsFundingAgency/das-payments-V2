using System;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.PeriodEnd.Messages.Events
{
    public interface IPeriodEndEvent: IPaymentsMessage
    {
        DateTimeOffset EventTime { get; }
        Guid EventId { get; }
        CollectionPeriod CollectionPeriod { get; }
    }
}