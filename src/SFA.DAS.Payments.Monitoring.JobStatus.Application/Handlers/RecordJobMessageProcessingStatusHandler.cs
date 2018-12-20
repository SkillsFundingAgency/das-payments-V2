using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.JobStatus.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Application.Handlers
{
    public class RecordJobMessageProcessingStatusHandler : IHandleMessages<RecordJobMessageProcessingStatus>
    {
        private readonly IPaymentLogger logger;
        private readonly IProviderEarningsJobService providerEarningsService;

        public RecordJobMessageProcessingStatusHandler(IPaymentLogger logger, IProviderEarningsJobService providerEarningsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.providerEarningsService = providerEarningsService ?? throw new ArgumentNullException(nameof(providerEarningsService));
        }

        public async Task Handle(RecordJobMessageProcessingStatus message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogVerbose($"Handling processed provider payment message. DC Job Id: {message.JobId}");
                await providerEarningsService.JobStepCompleted(message);
                logger.LogDebug($"Finished handling processed provider payment message. DC Job Id: {message.JobId}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error recording message processing status. Job id: {message.JobId}, message : {message.Id}. Error: {ex.Message}", ex);
                throw;
            }
        }
    }
}