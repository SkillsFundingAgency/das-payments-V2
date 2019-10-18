using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Audit.EarningEventsService.Handlers
{
    public class SubmissionSucceededHandler: IHandleMessages<SubmissionJobSucceeded>
    {
        private readonly IPaymentLogger logger;
        private readonly IEarningEventSubmissionSucceededProcessor processor;

        public SubmissionSucceededHandler(IPaymentLogger logger, IEarningEventSubmissionSucceededProcessor processor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public async Task Handle(SubmissionJobSucceeded message, IMessageHandlerContext context)
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