using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Application.JobContext;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Handlers
{
    public class SubmissionJobFinishedHandler : IHandleMessages<SubmissionJobFinishedEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IDasJobContextManagerService dasJobContextManagerService;


        public SubmissionJobFinishedHandler(IPaymentLogger logger, IDasJobContextManagerService dasJobContextManagerService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dasJobContextManagerService = dasJobContextManagerService;
        }

        public async Task Handle(SubmissionJobFinishedEvent message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Handling SubmissionJobFinished event for Ukprn: {message.Ukprn}");
            bool dasJobStatus;

            switch (message)
            {
                case SubmissionJobSucceeded _:
                    dasJobStatus = true;
                    break;
                case SubmissionJobFailed _:
                    dasJobStatus = false;
                    break;
                default:
                    throw new InvalidOperationException("Unable to resolve job status");
            }

            await dasJobContextManagerService.FinishProcessingJobContextMessage(dasJobStatus, message.JobId);

            logger.LogInfo($"Finished handling SubmissionJobFinished event for Ukprn: {message.Ukprn}");
        }
    }
}