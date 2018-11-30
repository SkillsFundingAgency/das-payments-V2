using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.Handlers
{
    public class FundingSourceEventHandler: IHandleMessages<FundingSourcePaymentEvent>
    {
        
        public Task Handle(FundingSourcePaymentEvent message, IMessageHandlerContext context)
        {
            throw new NotImplementedException();    
        }
    }
}