using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Handlers
{
    public class PeriodEndFcsHandoverSuccessHandler : IHandleMessages<PeriodEndFcsHandoverCompleteSucceeded>
    {
        public static ConcurrentBag<PeriodEndFcsHandoverCompleteSucceeded> ReceivedEvents { get; } =
            new ConcurrentBag<PeriodEndFcsHandoverCompleteSucceeded>();


        public Task Handle(PeriodEndFcsHandoverCompleteSucceeded message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}