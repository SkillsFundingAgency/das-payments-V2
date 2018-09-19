using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Handlers
{
    public class ApprenticeshipContractType2Handler:IHandleMessages<ApprenticeshipContractType2RequiredPaymentEvent>
    {
        public static List<ApprenticeshipContractType2RequiredPaymentEvent>  ReceivedEvents { get; } = new List<ApprenticeshipContractType2RequiredPaymentEvent>();

        public async Task Handle(ApprenticeshipContractType2RequiredPaymentEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine(message.ToJson());
            ReceivedEvents.Add(message);
        }
    }
}
