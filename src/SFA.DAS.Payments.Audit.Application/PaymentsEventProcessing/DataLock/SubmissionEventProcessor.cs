﻿using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.DataLock
{
    public interface ISubmissionEventProcessor
    {
        Task ProcessSubmissionFailedEvent(SubmissionJobFailed submissionFailedEvent);
        Task ProcessSubmissionSucceededEvent(SubmissionJobSucceeded submissionSucceededEvent);
    }

    public class SubmissionEventProcessor : ISubmissionEventProcessor
    {
        private readonly IDataLockEventRepository dataLockEventRepository;
        private readonly IPaymentsEventModelBatchService<DataLockEventModel> batchService;

        public SubmissionEventProcessor(IDataLockEventRepository dataLockEventRepository, IPaymentsEventModelBatchService<DataLockEventModel> batchService)
        {
            this.dataLockEventRepository = dataLockEventRepository;
            this.batchService = batchService;
        }

        public async Task ProcessSubmissionFailedEvent(SubmissionJobFailed submissionFailedEvent)
        {
            // flush audit service cache first
            await batchService.StorePayments(CancellationToken.None).ConfigureAwait(false);

            await dataLockEventRepository.DeleteEventsOfSubmission(
                submissionFailedEvent.Ukprn,
                submissionFailedEvent.AcademicYear,
                submissionFailedEvent.CollectionPeriod,
                submissionFailedEvent.IlrSubmissionDateTime
            ).ConfigureAwait(false);
        }

        public async Task ProcessSubmissionSucceededEvent(SubmissionJobSucceeded submissionSucceededEvent)
        {
            // flush audit service cache first
            await batchService.StorePayments(CancellationToken.None).ConfigureAwait(false);

            await dataLockEventRepository.DeleteEventsPriorToSubmission(
                submissionSucceededEvent.Ukprn,
                submissionSucceededEvent.AcademicYear,
                submissionSucceededEvent.CollectionPeriod,
                submissionSucceededEvent.IlrSubmissionDateTime
            ).ConfigureAwait(false);
        }
    }
}
