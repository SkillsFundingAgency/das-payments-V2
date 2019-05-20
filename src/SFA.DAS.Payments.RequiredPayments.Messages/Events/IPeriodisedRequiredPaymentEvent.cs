using System;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    // ReSharper disable once IdentifierTypo
    public interface IPeriodisedRequiredPaymentEvent : IPeriodisedPaymentEvent, IRequiredPaymentEvent
    {
        Guid EarningEventId { get; }
    }
}