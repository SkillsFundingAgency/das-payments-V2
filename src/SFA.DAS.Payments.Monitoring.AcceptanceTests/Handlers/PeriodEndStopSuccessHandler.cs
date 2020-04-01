using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Handlers
{
    public class PeriodEndStopSuccessHandler: IHandleMessages<PeriodEndStopJobSucceeded>
    {
        public static ConcurrentBag<PeriodEndStopJobSucceeded> ReceivedEvents { get; } =
            new ConcurrentBag<PeriodEndStopJobSucceeded>();


        public Task Handle(PeriodEndStopJobSucceeded message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}