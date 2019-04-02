using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;

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
