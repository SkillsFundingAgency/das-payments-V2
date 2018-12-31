using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Application.Handlers
{
    public class StartedProcessingProviderEarningsEventHandler : IHandleMessages<RecordStartedProcessingProviderEarningsJob>
    {
        private readonly IPaymentLogger logger;
        private readonly IProviderEarningsJobService providerEarningsService;

        public StartedProcessingProviderEarningsEventHandler(IPaymentLogger logger, IProviderEarningsJobService providerEarningsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.providerEarningsService = providerEarningsService ?? throw new ArgumentNullException(nameof(providerEarningsService));
        }

        public async Task Handle(RecordStartedProcessingProviderEarningsJob message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogVerbose($"Handling provider earnings job. Job Id: {message.JobId}.");
                await providerEarningsService.JobStarted(message);
                logger.LogDebug($"Finished handling provider earnings job. Job Id: {message.JobId}.");

            }
            catch (Exception ex)
            {
                logger.LogError($"Error recording new provider earnings job. Job id: {message.JobId}, ukprn: {message.Ukprn}. Error: {ex.Message}", ex);
                throw;
            }
        }
    }
}