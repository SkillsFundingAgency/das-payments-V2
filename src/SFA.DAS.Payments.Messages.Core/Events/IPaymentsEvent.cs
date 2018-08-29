using System;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public interface IPaymentsEvent : IPaymentsMessage
    {
        DateTimeOffset EventTime { get; }
    }
}