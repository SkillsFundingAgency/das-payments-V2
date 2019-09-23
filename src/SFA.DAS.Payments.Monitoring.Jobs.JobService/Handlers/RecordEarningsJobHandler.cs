using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers
{
    public class RecordEarningsJobHandler: IHandleMessages<RecordEarningsJob>
    {
        private readonly IPaymentLogger logger;
        private readonly IUnitOfWorkScopeFactory scopeFactory;
        private readonly IJobStatusManager jobStatusManager;

        public RecordEarningsJobHandler(IPaymentLogger logger, IUnitOfWorkScopeFactory scopeFactory, IJobStatusManager jobStatusManager)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            this.jobStatusManager = jobStatusManager ?? throw new ArgumentNullException(nameof(jobStatusManager));
        }

        public async Task Handle(RecordEarningsJob message, IMessageHandlerContext context)
        {

            using (var scope = scopeFactory.Create("JobService.RecordEarningsJob"))
            {
                try
                {
                    var earningsJobService = scope.Resolve<IEarningsJobService>();
                    await earningsJobService.RecordNewJob(message, CancellationToken.None).ConfigureAwait(false);
                    await scope.Commit();
                }
                catch (Exception ex)
                {
                    scope.Abort();
                    logger.LogWarning($"Failed to record earnings job: {message.JobId}. Error: {ex.Message}, {ex}");
                    throw;
                }
            }
            jobStatusManager.StartMonitoringJob(message.JobId, JobType.EarningsJob);
        }
    }
}