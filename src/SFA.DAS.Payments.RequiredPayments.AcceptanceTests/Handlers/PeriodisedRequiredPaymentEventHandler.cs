﻿using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Handlers
{
    public class PeriodisedRequiredPaymentEventHandler : IHandleMessages<PeriodisedRequiredPaymentEvent>
    {
        public static ConcurrentBag<PeriodisedRequiredPaymentEvent> ReceivedEvents { get; } = new ConcurrentBag<PeriodisedRequiredPaymentEvent>();

        public Task Handle(PeriodisedRequiredPaymentEvent message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}
