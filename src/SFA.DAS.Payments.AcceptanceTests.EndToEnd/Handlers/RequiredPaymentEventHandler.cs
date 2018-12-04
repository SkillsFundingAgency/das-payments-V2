using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    public class RequiredPaymentEventHandler : IHandleMessages<RequiredPaymentEvent>
    {
        public static ConcurrentBag<RequiredPaymentEvent> ReceivedEvents { get; } = new ConcurrentBag<RequiredPaymentEvent>();

        public Task Handle(RequiredPaymentEvent message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;            
        }
    }
}