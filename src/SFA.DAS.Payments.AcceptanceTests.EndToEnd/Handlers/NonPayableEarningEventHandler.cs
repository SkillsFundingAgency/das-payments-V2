using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    public class NonPayableEarningEventHandler : IHandleMessages<NonPayableEarningEvent>
    {
        public static ConcurrentBag<NonPayableEarningEvent> ReceivedEvents { get; } = new ConcurrentBag<NonPayableEarningEvent>();

        public Task Handle(NonPayableEarningEvent message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}
