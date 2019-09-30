using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers
{
    public class RecordEarningsJobHandler : IHandleMessages<RecordEarningsJob>
    {
        private readonly IPaymentLogger logger;
        private readonly IEarningsJobService earningsJobService;

        public RecordEarningsJobHandler(IPaymentLogger logger, IEarningsJobService earningsJobService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.earningsJobService = earningsJobService ?? throw new ArgumentNullException(nameof(earningsJobService));
        }

        public async Task Handle(RecordEarningsJob message, IMessageHandlerContext context)
        {
            try
            {
                await earningsJobService.RecordNewJob(message, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to record earnings job: {message.JobId}. Error: {ex.Message}, {ex}");
                throw;
            }
        }
    }
}