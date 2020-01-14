using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Handlers
{
    public class SubmissionJobSucceededHandler: IHandleMessages<SubmissionJobSucceeded>
    {
        public static ConcurrentBag<SubmissionJobSucceeded> ReceivedEvents { get; } = new ConcurrentBag<SubmissionJobSucceeded>();

        public Task Handle(SubmissionJobSucceeded message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}