using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.DasHandlers
{
  public  class RecordedAct1CompletionPaymentEventHandler : IHandleMessages<RecordedAct1CompletionPayment>
    {

        public static ConcurrentBag<RecordedAct1CompletionPayment> ReceivedEvents { get; } = new ConcurrentBag<RecordedAct1CompletionPayment>();


        public Task Handle(RecordedAct1CompletionPayment message, IMessageHandlerContext context)
        {
            Console.WriteLine(message.ToJson());
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}
