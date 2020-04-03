using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService.Handlers
{
    public class Act1RedundancyEarningEventHandler : IHandleMessages<ApprenticeshipContractType1EarningEvent>
    {
        public Task Handle(ApprenticeshipContractType1EarningEvent message, IMessageHandlerContext context)
        {
            throw new System.NotImplementedException();
            //mimic Datalocks here and produce payable earning events then send local
        }
    }
}