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
    public class
        RecordPeriodEndFcsHandOverCompleteJobHandler : IHandleMessageBatches<RecordPeriodEndFcsHandOverCompleteJob>
    {
        private readonly IGenericPeriodEndJobStatusManager jobStatusManager;
        private readonly IPaymentLogger logger;
        private readonly IPeriodEndJobService periodEndJobService;
        private readonly IPeriodEndArchiverClient periodEndArchiverClient;

        public RecordPeriodEndFcsHandOverCompleteJobHandler(IPaymentLogger logger,
            IPeriodEndJobService periodEndJobService,
            IGenericPeriodEndJobStatusManager jobStatusManager,
            IPeriodEndArchiverClient periodEndArchiverClient)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndJobService =
                periodEndJobService ?? throw new ArgumentNullException(nameof(periodEndJobService));
            this.jobStatusManager = jobStatusManager ?? throw new ArgumentNullException(nameof(jobStatusManager));
            this.periodEndArchiverClient = periodEndArchiverClient;
            this.periodEndArchiverClient = periodEndArchiverClient;
        }

        public async Task Handle(IList<RecordPeriodEndFcsHandOverCompleteJob> messages,
            CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                logger.LogInfo($"Handling period end Fcs handover job: {message.ToJson()}");
                await periodEndJobService.RecordPeriodEndJob(message, cancellationToken);
                jobStatusManager.StartMonitoringJob(message.JobId, JobType.PeriodEndFcsHandOverCompleteJob);

                // Poll Archive Status 
                var archiveFinished = false;
                while (!archiveFinished)
                {
                    archiveFinished = await periodEndArchiverClient.ArchiveStatus();
                }

                logger.LogInfo($"Handled period end Fcs handover job: {message.JobId}");
            }
        }
    }
}