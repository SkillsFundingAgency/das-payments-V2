using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.PeriodEnd
{
    public class
        RecordPeriodEndFcsHandOverCompleteJobHandler : IHandleMessageBatches<RecordPeriodEndFcsHandOverCompleteJob>
    {
        private readonly IPeriodEndArchiveJobStatusManager jobStatusManager;
        private readonly IPaymentLogger logger;
        private readonly IPeriodEndJobService periodEndJobService;


        public RecordPeriodEndFcsHandOverCompleteJobHandler(
            IPaymentLogger logger,
            IPeriodEndJobService periodEndJobService,
            IPeriodEndArchiveJobStatusManager jobStatusManager)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndJobService =
                periodEndJobService ?? throw new ArgumentNullException(nameof(periodEndJobService));
            this.jobStatusManager =
                jobStatusManager ?? throw new ArgumentNullException(nameof(jobStatusManager));
        }

        public async Task Handle(IList<RecordPeriodEndFcsHandOverCompleteJob> messages,
            CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                logger.LogInfo($"Handling period end FCS handover job: {message.ToJson()}");
                await periodEndJobService.RecordPeriodEndJob(message, cancellationToken);
                await jobStatusManager.StartArchive(message);
                jobStatusManager.StartArchiveMonitoringJob(message.JobId, JobType.PeriodEndFcsHandOverCompleteJob);

                logger.LogInfo($"Handling period end FCS handover job: {message.JobId}");
            }
        }
    }
}