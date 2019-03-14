using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Handlers
{
    public class ApprenticeshipContractType2Handler:IHandleMessages<CalculatedRequiredCoInvestedAmount>
    {
        //public static List<ApprenticeshipContractType2RequiredPaymentEvent>  ReceivedEvents { get; } = new List<ApprenticeshipContractType2RequiredPaymentEvent>();
        public static ConcurrentBag<CalculatedRequiredCoInvestedAmount> ReceivedEvents { get; } = new ConcurrentBag<CalculatedRequiredCoInvestedAmount>();

        public Task Handle(CalculatedRequiredCoInvestedAmount message, IMessageHandlerContext context)
        {
            Console.WriteLine(message.ToJson());
            ReceivedEvents.Add(message);
            return Task.FromResult(0);
        }
    }
}
