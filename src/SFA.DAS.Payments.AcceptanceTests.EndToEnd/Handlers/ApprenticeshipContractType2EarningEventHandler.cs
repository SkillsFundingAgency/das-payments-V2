using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    public class ApprenticeshipContractType2EarningEventHandler : IHandleMessages<ApprenticeshipContractType2EarningEvent>
    {
        public static ConcurrentBag<ApprenticeshipContractType2EarningEvent> ReceivedEvents { get; } = new ConcurrentBag<ApprenticeshipContractType2EarningEvent>();

        public Task Handle(ApprenticeshipContractType2EarningEvent message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.FromResult(0);
        }
    }
}
