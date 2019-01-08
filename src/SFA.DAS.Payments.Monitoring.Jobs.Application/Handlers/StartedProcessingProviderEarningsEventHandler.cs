using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Handlers
{
    public class StartedProcessingProviderEarningsEventHandler : IHandleMessages<RecordStartedProcessingProviderEarningsJob>
    {
        private readonly IPaymentLogger logger;
        private readonly IEarningsJobService earningsService;

        public StartedProcessingProviderEarningsEventHandler(IPaymentLogger logger, IEarningsJobService earningsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.earningsService = earningsService ?? throw new ArgumentNullException(nameof(earningsService));
        }

        public async Task Handle(RecordStartedProcessingProviderEarningsJob message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogVerbose($"Handling provider earnings job. Job Id: {message.JobId}.");
                await earningsService.JobStarted(message);
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