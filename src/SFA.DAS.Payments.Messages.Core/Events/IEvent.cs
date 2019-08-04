using System;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public interface IEvent: IPaymentsMessage
    {
        Guid EventId { get; }
        DateTimeOffset EventTime { get; }
    }
}