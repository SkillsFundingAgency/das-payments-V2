using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Handlers
{
    public class SubmissionJobFailedHandler : IHandleMessages<SubmissionJobFailed>
    {
        public static ConcurrentBag<SubmissionJobFailed> ReceivedEvents { get; } = new ConcurrentBag<SubmissionJobFailed>();

        public Task Handle(SubmissionJobFailed message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}