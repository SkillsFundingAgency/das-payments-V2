using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    public class ApprenticeshipContractType2RequiredPaymentEventHandler : IHandleMessages<ApprenticeshipContractType2RequiredPaymentEvent>
    {
        public static ConcurrentBag<ApprenticeshipContractType2RequiredPaymentEvent> ReceivedEvents { get; } = new ConcurrentBag<ApprenticeshipContractType2RequiredPaymentEvent>();

        public Task Handle(ApprenticeshipContractType2RequiredPaymentEvent message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.FromResult(0);            
        }
    }
}