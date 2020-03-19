using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Handlers
{
  public  class RecordedAct1CompletionPaymentEventHandler : IHandleMessages<RecordedAct1CompletionPaymentEvent>
    {

        public static ConcurrentBag<RecordedAct1CompletionPaymentEvent> ReceivedEvents { get; } = new ConcurrentBag<RecordedAct1CompletionPaymentEvent>();


        public async Task Handle(RecordedAct1CompletionPaymentEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine(message.ToJson());
            ReceivedEvents.Add(message);
            await Task.CompletedTask;
        }
    }
}
