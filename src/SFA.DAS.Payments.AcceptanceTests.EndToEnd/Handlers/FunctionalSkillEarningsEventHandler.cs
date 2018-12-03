using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    public class FunctionalSkillEarningsEventHandler : IHandleMessages<FunctionalSkillEarningsEvent>
    {
        public static ConcurrentBag<FunctionalSkillEarningsEvent> ReceivedEvents { get; } = new ConcurrentBag<FunctionalSkillEarningsEvent>();

        public Task Handle(FunctionalSkillEarningsEvent message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}
