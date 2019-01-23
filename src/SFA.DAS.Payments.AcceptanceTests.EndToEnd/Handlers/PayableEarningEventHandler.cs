using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.DataLocks.Messages;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    public class PayableEarningEventHandler : IHandleMessages<PayableEarning>
    {
        public static ConcurrentBag<PayableEarning> ReceivedEvents { get; } = new ConcurrentBag<PayableEarning>();

        public Task Handle(PayableEarning message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}