using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.Earnings;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.Submission
{
    public class RecordEarningsJobBatchHandler : IHandleMessageBatches<RecordEarningsJob>
    {
        private readonly IPaymentLogger logger;
        private readonly IEarningsJobService earningsJobService;
        private readonly IJobStatusManager jobStatusManager;

        public RecordEarningsJobBatchHandler(IPaymentLogger logger, IEarningsJobService earningsJobService, IEarningsJobStatusManager jobStatusManager)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.earningsJobService = earningsJobService ?? throw new ArgumentNullException(nameof(earningsJobService));
            this.jobStatusManager = jobStatusManager ?? throw new ArgumentNullException(nameof(jobStatusManager));
        }

        public async Task Handle(IList<RecordEarningsJob> messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await earningsJobService.RecordNewJob(message, CancellationToken.None).ConfigureAwait(false);
                jobStatusManager.StartMonitoringJob(message.JobId, JobType.EarningsJob);
            }
        }
    }
}