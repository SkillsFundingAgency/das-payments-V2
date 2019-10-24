using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.DataLock;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.Audit.DataLockService.Handlers
{
    public class SubmissionFailedEventHandler : IHandleMessages<SubmissionFailedEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly ISubmissionEventProcessor submissionEventProcessor;

        public SubmissionFailedEventHandler(ISubmissionEventProcessor submissionEventProcessor, IPaymentLogger logger)
        {
            this.submissionEventProcessor = submissionEventProcessor;
            this.logger = logger;
        }

        public async Task Handle(SubmissionFailedEvent message, IMessageHandlerContext context)
        {
            var logString = $"{typeof(SubmissionFailedEvent).Name}. UKPRN: {message.Ukprn} {message.AcademicYear}-R{message.CollectionPeriod:D2}, ILR Submission: {message.IlrSubmissionDateTime:s}, Job ID: {message.JobId}";

            logger.LogInfo("Handling " + logString);

            try
            {
                await submissionEventProcessor.ProcessSubmissionFailedEvent(message).ConfigureAwait(false);
                logger.LogInfo("Finished handling " + logString);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling {logString}. {ex.Message}");
                throw;
            }
        }
    }
}
