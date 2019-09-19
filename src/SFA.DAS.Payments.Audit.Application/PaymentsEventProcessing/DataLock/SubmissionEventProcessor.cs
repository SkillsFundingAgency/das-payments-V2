using System.Threading.Tasks;
using SFA.DAS.Payments.Audit.Application.Data;
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

        public SubmissionEventProcessor(IDataLockEventRepository dataLockEventRepository)
        {
            this.dataLockEventRepository = dataLockEventRepository;
        }

        public async Task ProcessSubmissionFailedEvent(SubmissionFailedEvent submissionFailedEvent)
        {
            await dataLockEventRepository.DeleteEventsOfSubmission(
                submissionFailedEvent.Ukprn,
                submissionFailedEvent.AcademicYear,
                submissionFailedEvent.CollectionPeriod,
                submissionFailedEvent.IlrSubmissionDateTime
            ).ConfigureAwait(false);
        }

        public async Task ProcessSubmissionSucceededEvent(SubmissionSucceededEvent submissionSucceededEvent)
        {
            await dataLockEventRepository.DeleteEventsPriorToSubmission(
                submissionSucceededEvent.Ukprn,
                submissionSucceededEvent.AcademicYear,
                submissionSucceededEvent.CollectionPeriod,
                submissionSucceededEvent.IlrSubmissionDateTime
            ).ConfigureAwait(false);
        }
    }
}
