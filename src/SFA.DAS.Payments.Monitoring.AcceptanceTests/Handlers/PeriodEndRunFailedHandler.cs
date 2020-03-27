using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Handlers
{
    public class PeriodEndRunFailedHandler: IHandleMessages<PeriodEndRunJobFailed>
    {
        public static ConcurrentBag<PeriodEndRunJobFailed> ReceivedEvents { get; } =
            new ConcurrentBag<PeriodEndRunJobFailed>();


        public Task Handle(PeriodEndRunJobFailed message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}