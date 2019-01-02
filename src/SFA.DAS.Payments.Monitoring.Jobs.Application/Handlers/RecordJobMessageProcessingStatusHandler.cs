using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Handlers
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
                logger.LogVerbose($"Handling job message processed. DC Job Id: {message.JobId}, message name: {message.MessageName}, id: {message.Id}");
                await providerEarningsService.JobStepCompleted(message);
                logger.LogDebug($"Finished handling job message processed. DC Job Id: {message.JobId}, message name: {message.MessageName}, id: {message.Id}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error recording message processing status. Job id: {message.JobId}, message : {message.Id}, message name: {message.MessageName}. Error: {ex.Message}", ex);
                throw;
            }
        }
    }
}