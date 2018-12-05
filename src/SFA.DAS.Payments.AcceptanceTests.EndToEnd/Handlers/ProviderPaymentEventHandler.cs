﻿using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    public class ProviderPaymentEventHandler : IHandleMessages<ProviderPaymentEvent>
    {
        public static ConcurrentBag<ProviderPaymentEvent> ReceivedEvents { get; } = new ConcurrentBag<ProviderPaymentEvent>();

        public Task Handle(ProviderPaymentEvent message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}