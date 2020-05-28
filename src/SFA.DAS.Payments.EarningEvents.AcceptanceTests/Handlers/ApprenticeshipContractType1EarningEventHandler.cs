using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Handlers
{
    public class ApprenticeshipContractType1EarningEventHandler : IHandleMessages<ApprenticeshipContractType1EarningEvent>
    {

        public static ConcurrentBag<ApprenticeshipContractType1EarningEvent> ReceivedEvents { get; } = new ConcurrentBag<ApprenticeshipContractType1EarningEvent>();

        public Task Handle(ApprenticeshipContractType1EarningEvent message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}