using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using System.Linq;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    public class EarningEventHandler : IHandleMessages<EarningEvent>
    {
        public static ConcurrentBag<EarningEvent> ReceivedEvents { get; } = new ConcurrentBag<EarningEvent>();

        public Task Handle(EarningEvent message, IMessageHandlerContext context)
        {

            if (message is FunctionalSkillEarningsEvent functionalSkillsEvent)
            {
                if (functionalSkillsEvent.Earnings.All(e => e.Periods.All(p => p.Amount == 0)))
                {
                    return Task.CompletedTask;
                }
            }

            if (message is ApprenticeshipContractType2EarningEvent apprenticeshipEvent)
            {
                if (apprenticeshipEvent.IncentiveEarnings.All(e => e.Periods.All(p => p.Amount == 0)) && 
                    apprenticeshipEvent.OnProgrammeEarnings.All(e=>e.Periods.All(p=>p.Amount == 0)))
                {
                    return Task.CompletedTask;
                }
            }

            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}
