using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.DataLock
{
    public interface ISubmissionEventProcessor
    {
        Task ProcessSubmissionFailedEvent(SubmissionFailedEvent submissionFailedEvent);
        Task ProcessSubmissionSucceededEvent(SubmissionSucceededEvent submissionSucceededEvent);
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

        public async Task ProcessSubmissionFailedEvent(SubmissionFailedEvent submissionFailedEvent)
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

        public async Task ProcessSubmissionSucceededEvent(SubmissionSucceededEvent submissionSucceededEvent)
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
