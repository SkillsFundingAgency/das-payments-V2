using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Handlers
{
    public class PeriodEndStartFailedHandler : IHandleMessages<PeriodEndStartJobFailed>
    {
        public static ConcurrentBag<PeriodEndStartJobFailed> ReceivedEvents { get; } =
            new ConcurrentBag<PeriodEndStartJobFailed>();


        public Task Handle(PeriodEndStartJobFailed message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}