using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Audit.EarningEventsService.Handlers
{
    public class SubmissionFailedHandler : IHandleMessages<SubmissionJobFailed>
    {
        private readonly IPaymentLogger logger;
        private readonly IEarningEventSubmissionFailedProcessor processor;

        public SubmissionFailedHandler(IPaymentLogger logger, IEarningEventSubmissionFailedProcessor processor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public async Task Handle(SubmissionJobFailed message, IMessageHandlerContext context)
        {
            try
            {
                await processor.Process(message, CancellationToken.None).ConfigureAwait(false);
                logger.LogInfo($"Finished processing submission succeeded event for earning events. message: {message.ToJson()}");
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to remove previous submission earning events.  Error: {ex.Message}.  Ukprn: {message.Ukprn}, Collection period: {message.AcademicYear}-{message.CollectionPeriod}, Failed job: {message.JobId}.");
                throw;
            }
        }
    }
}