using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class FundingSourcePaymentEventBatchHandler: IHandleMessageBatches<FundingSourcePaymentEvent>
    {
        public Task Handle(IList<FundingSourcePaymentEvent> messages, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}