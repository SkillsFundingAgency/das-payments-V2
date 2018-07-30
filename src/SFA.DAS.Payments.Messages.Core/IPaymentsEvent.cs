using System;

namespace SFA.DAS.Payments.Messages.Core
{
    public interface IPaymentsEvent : IPaymentsMessage
    {
        DateTimeOffset EventTime { get; }
    }
}