using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Handlers
{
    public class PeriodEndStopFailedHandler: IHandleMessages<PeriodEndStopJobFailed>
    {
        public static ConcurrentBag<PeriodEndStopJobFailed> ReceivedEvents { get; } =
            new ConcurrentBag<PeriodEndStopJobFailed>();


        public Task Handle(PeriodEndStopJobFailed message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}