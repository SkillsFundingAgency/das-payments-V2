using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.PeriodEnd
{
    public class RecordPeriodEndRunJobHandler : IHandleMessageBatches<RecordPeriodEndRunJob>
    {
        private readonly IPaymentLogger logger;
        private readonly IPeriodEndJobService periodEndJobService;
        private readonly IJobStatusManager jobStatusManager;

        public RecordPeriodEndRunJobHandler(IPaymentLogger logger, IPeriodEndJobService periodEndJobService, IPeriodEndJobStatusManager jobStatusManager)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndJobService = periodEndJobService ?? throw new ArgumentNullException(nameof(periodEndJobService));
            this.jobStatusManager = jobStatusManager ?? throw new ArgumentNullException(nameof(jobStatusManager));
        }

        public async Task Handle(IList<RecordPeriodEndRunJob> messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                logger.LogInfo($"Handling period end run job: {message.ToJson()}");
                await periodEndJobService.RecordPeriodEndJob(message,  cancellationToken);
                jobStatusManager.StartMonitoringJob(message.JobId, JobType.PeriodEndRunJob);
                logger.LogInfo($"Handled period end run job: {message.JobId}");
            }
        }
    }
}