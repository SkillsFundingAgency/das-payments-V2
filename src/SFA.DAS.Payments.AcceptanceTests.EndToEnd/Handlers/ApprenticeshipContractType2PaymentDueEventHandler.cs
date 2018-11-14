using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    public class ApprenticeshipContractType2PaymentDueEventHandler : IHandleMessages<ApprenticeshipContractType2PaymentDueEvent>
    {
        public static ConcurrentBag<ApprenticeshipContractType2PaymentDueEvent> ReceivedEvents { get; } = new ConcurrentBag<ApprenticeshipContractType2PaymentDueEvent>();

        public Task Handle(ApprenticeshipContractType2PaymentDueEvent message, IMessageHandlerContext context)
        {
            //Console.WriteLine($"Received event type: {message.GetType().Name}, payload: {message.ToJson()}");
            ReceivedEvents.Add(message);
            return Task.FromResult(0);
        }
    }
}