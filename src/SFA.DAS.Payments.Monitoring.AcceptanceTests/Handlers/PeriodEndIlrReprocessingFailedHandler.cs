using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Handlers
{
    public class PeriodEndIlrReprocessingFailedHandler : IHandleMessages<PeriodEndIlrReprocessingFailed>
    {
        public static ConcurrentBag<PeriodEndIlrReprocessingFailed> ReceivedEvents { get; } =
            new ConcurrentBag<PeriodEndIlrReprocessingFailed>();


        public Task Handle(PeriodEndIlrReprocessingFailed message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}