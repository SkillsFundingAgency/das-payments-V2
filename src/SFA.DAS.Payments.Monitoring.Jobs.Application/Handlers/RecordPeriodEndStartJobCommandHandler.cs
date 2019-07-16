using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Handlers
{
    public class RecordPeriodEndStartJobCommandHandler : IHandleMessages<RecordPeriodEndJob>
    {
        private readonly IPaymentLogger logger;
        private readonly IMonthEndJobService monthEndJobService;

        public RecordPeriodEndStartJobCommandHandler(IPaymentLogger logger, IMonthEndJobService monthEndJobService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.monthEndJobService = monthEndJobService ?? throw new ArgumentNullException(nameof(monthEndJobService));
        }

        public async Task Handle(RecordPeriodEndJob message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogVerbose($"Handling month end job. Job Id: {message.JobId}, collection period: {message.CollectionYear}-{message.CollectionPeriod} ");
                await monthEndJobService.RecordPeriodEndJob(message);
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