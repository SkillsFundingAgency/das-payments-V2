using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers
{
    public class RecordEarningsJobHandler : IHandleMessages<RecordEarningsJob>
    {
        private readonly IPaymentLogger logger;
        private readonly IEarningsJobService earningsJobService;
        private readonly IJobStatusManager jobStatusManager;


        public RecordEarningsJobHandler(IPaymentLogger logger, IEarningsJobService earningsJobService, IJobStatusManager jobStatusManager)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.earningsJobService = earningsJobService ?? throw new ArgumentNullException(nameof(earningsJobService));
            this.jobStatusManager = jobStatusManager ?? throw new ArgumentNullException(nameof(jobStatusManager));
        }

        public async Task Handle(RecordEarningsJob message, IMessageHandlerContext context)
        {
            try
            {
                //await earningsJobService.RecordNewJob(message, CancellationToken.None).ConfigureAwait(false);
                //jobStatusManager.StartMonitoringJob(message.JobId, JobType.EarningsJob);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to record earnings job: {message.JobId}. Error: {ex.Message}, {ex}");
                throw;
            }
        }
    }
}