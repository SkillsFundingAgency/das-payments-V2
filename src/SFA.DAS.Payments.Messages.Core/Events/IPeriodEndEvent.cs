using System;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public interface IPeriodEndEvent : IPaymentsMessage
    {
        DateTimeOffset EventTime { get; }
        Guid EventId { get; }
        CollectionPeriod CollectionPeriod { get; }
    }
}