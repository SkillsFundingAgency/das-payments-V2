using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Application.JobContext;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Handlers
{
    public class SubmissionJobFinishedHandler : IHandleMessages<DasSubmissionJobFinishedEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IDasJobContextManagerService dasJobContextManagerService;


        public SubmissionJobFinishedHandler(IPaymentLogger logger, IDasJobContextManagerService dasJobContextManagerService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dasJobContextManagerService = dasJobContextManagerService;
        }

        public async Task Handle(DasSubmissionJobFinishedEvent message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Handling DasSubmissionJobFinishedEvent event for Ukprn: {message.Ukprn}");
            try
            {
                bool dasJobStatus;

                switch (message)
                {
                    case DasSubmissionJobSucceeded _:
                        dasJobStatus = true;
                        break;
                    case DasSubmissionJobFailed _:
                        dasJobStatus = false;
                        break;
                    default:
                        throw new InvalidOperationException("Unable to resolve job status");
                }

                await dasJobContextManagerService.FinishProcessingJobContextMessage(dasJobStatus, message.JobId);
            }
            catch (Exception exception)
            {
                logger.LogError($"Error handling DasSubmissionJobFinished event for Ukprn: {message.Ukprn}", exception);
                throw;
            }

            logger.LogInfo($"Finished handling DasSubmissionJobFinished event for Ukprn: {message.Ukprn}");
        }
    }
}