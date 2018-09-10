using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Handlers
{
    public class ApprenticeshipContractType2Handler:IHandleMessages<ApprenticeshipContractType1EarningEvent>
    {
        public async Task Handle(ApprenticeshipContractType1EarningEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine(message.ToJson());
        }
    }
}
