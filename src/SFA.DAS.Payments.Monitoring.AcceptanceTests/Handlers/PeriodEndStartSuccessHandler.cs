using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Handlers
{
    public class PeriodEndStartSuccessHandler: IHandleMessages<PeriodEndStartJobSucceeded>
    {
        public static ConcurrentBag<PeriodEndStartJobSucceeded> ReceivedEvents { get; } = new ConcurrentBag<PeriodEndStartJobSucceeded>();


        public Task Handle(PeriodEndStartJobSucceeded message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}