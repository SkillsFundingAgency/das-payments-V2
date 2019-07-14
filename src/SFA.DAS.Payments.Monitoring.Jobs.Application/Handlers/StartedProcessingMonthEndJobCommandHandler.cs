using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Handlers
{
    public class StartedProcessingMonthEndJobCommandHandler : IHandleMessages<RecordPeriodEndStartJob>
    {
        private readonly IPaymentLogger logger;
        private readonly IMonthEndJobService monthEndJobService;

        public StartedProcessingMonthEndJobCommandHandler(IPaymentLogger logger, IMonthEndJobService monthEndJobService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.monthEndJobService = monthEndJobService ?? throw new ArgumentNullException(nameof(monthEndJobService));
        }

        public async Task Handle(RecordPeriodEndStartJob message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogVerbose($"Handling month end job. Job Id: {message.JobId}, collection period: {message.CollectionYear}-{message.CollectionPeriod} ");
                await monthEndJobService.JobStarted(message);
                logger.LogDebug($"Finished handling month end job. Job Id: {message.JobId}, collection period: {message.CollectionYear}-{message.CollectionPeriod}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error recording new month end job. Job id: {message.JobId}, collection period: {message.CollectionYear}-{message.CollectionPeriod}. Error: {ex.Message}", ex);
                throw;
            }
        }
    }
}