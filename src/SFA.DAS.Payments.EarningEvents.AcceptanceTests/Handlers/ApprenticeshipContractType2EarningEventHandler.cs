using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Handlers
{
    public class ApprenticeshipContractType2EarningEventHandler : IHandleMessages<ApprenticeshipContractType2EarningEvent>
    {
        public static ConcurrentBag<ApprenticeshipContractType2EarningEvent> ReceivedEvents { get; } = new ConcurrentBag<ApprenticeshipContractType2EarningEvent>();

        public async Task Handle(ApprenticeshipContractType2EarningEvent message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            await Task.FromResult(0);
        }
    }
}