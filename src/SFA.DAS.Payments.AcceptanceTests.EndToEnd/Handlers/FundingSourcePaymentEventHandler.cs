using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    public class FundingSourcePaymentEventHandler : IHandleMessages<FundingSourcePaymentEvent>
    {
        public static ConcurrentBag<FundingSourcePaymentEvent> ReceivedEvents { get; } = new ConcurrentBag<FundingSourcePaymentEvent>();

        public Task Handle(FundingSourcePaymentEvent message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}
