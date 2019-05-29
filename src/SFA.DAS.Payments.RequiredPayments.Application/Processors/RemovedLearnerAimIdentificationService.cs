using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public class RemovedLearnerAimIdentificationService : IRemovedLearnerAimIdentificationService
    {
        private readonly IPaymentHistoryRepository paymentHistoryRepository;

        public RemovedLearnerAimIdentificationService(IPaymentHistoryRepository paymentHistoryRepository)
        {
            this.paymentHistoryRepository = paymentHistoryRepository;
        }

        public async Task<List<IdentifiedRemovedLearningAim>> IdentifyRemovedLearnerAims(short academicYear, byte collectionPeriod, long ukprn, DateTime ilrSubmissionDateTime, CancellationToken cancellationToken)
        {
            return await paymentHistoryRepository.IdentifyRemovedLearnerAims(academicYear, collectionPeriod, ukprn, ilrSubmissionDateTime, cancellationToken).ConfigureAwait(false);
        }
    }
}