namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers
{
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using NServiceBus;
    using SFA.DAS.Payments.EarningEvents.Messages.Events;

    public class EarningEventHandler : IHandleMessages<EarningEvent>
    {
        public static ConcurrentBag<EarningEvent> ReceivedEvents { get; } = new ConcurrentBag<EarningEvent>();

        public Task Handle(EarningEvent message, IMessageHandlerContext context)
        {
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}
