using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Handlers
{
    public class PeriodEndRunSuccessHandler: IHandleMessages<PeriodEndRunJobSucceeded>
    {
        public static ConcurrentBag<PeriodEndRunJobSucceeded> ReceivedEvents { get; } =
            new ConcurrentBag<PeriodEndRunJobSucceeded>();


        public Task Handle(PeriodEndRunJobSucceeded message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}