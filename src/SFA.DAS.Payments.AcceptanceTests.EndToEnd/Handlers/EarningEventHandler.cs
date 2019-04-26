using NServiceBus;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    public class EarningEventHandler : IHandleMessages<EarningEvent>
    {
        public static ConcurrentBag<EarningEvent> ReceivedEvents { get; } = new ConcurrentBag<EarningEvent>();

        public Task Handle(EarningEvent message, IMessageHandlerContext context)
        {
            if (message is FunctionalSkillEarningsEvent @event && !@event.Earnings.Any())
            {
                return Task.CompletedTask;
            }

            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}
