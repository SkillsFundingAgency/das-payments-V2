using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public class RefundRemovedLearningAimProcessor: IRefundRemovedLearningAimProcessor
    {
        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> RefundLearningAim(IdentifiedRemovedLearningAim identifiedRemovedLearningAim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}