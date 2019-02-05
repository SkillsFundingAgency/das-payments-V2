using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.DataLocks.Messages;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    public class PayableEarningEventHandler : IHandleMessages<PayableEarningEvent>
    {
        public static ConcurrentBag<PayableEarningEvent> ReceivedEvents { get; } = new ConcurrentBag<PayableEarningEvent>();

        public Task Handle(PayableEarningEvent message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}