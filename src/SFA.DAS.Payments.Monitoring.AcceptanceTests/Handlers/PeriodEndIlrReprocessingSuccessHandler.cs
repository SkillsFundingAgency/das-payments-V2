using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Handlers
{
    public class PeriodEndIlrReprocessingSuccessHandler : IHandleMessages<PeriodEndIlrReprocessingSucceeded>
    {
        public static ConcurrentBag<PeriodEndIlrReprocessingSucceeded> ReceivedEvents { get; } =
            new ConcurrentBag<PeriodEndIlrReprocessingSucceeded>();


        public Task Handle(PeriodEndIlrReprocessingSucceeded message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}