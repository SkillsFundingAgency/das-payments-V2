using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers.PeriodEnd
{
    public class RecordPeriodEndSubmissionWindowValidationJobHandler : IHandleMessageBatches<RecordPeriodEndSubmissionWindowValidationJob>
    {
        private readonly IPaymentLogger logger;
        private readonly IPeriodEndJobService periodEndJobService;
        private readonly IPeriodEndSubmissionWindowValidationJobStatusManager periodEndSubmissionWindowValidationJobStatusManager;

        public RecordPeriodEndSubmissionWindowValidationJobHandler(IPaymentLogger logger, IPeriodEndJobService periodEndJobService, IPeriodEndSubmissionWindowValidationJobStatusManager periodEndSubmissionWindowValidationJobStatusManager)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndJobService = periodEndJobService ?? throw new ArgumentNullException(nameof(periodEndJobService));
            this.periodEndSubmissionWindowValidationJobStatusManager = periodEndSubmissionWindowValidationJobStatusManager ?? throw new ArgumentNullException(nameof(periodEndSubmissionWindowValidationJobStatusManager));
        }

        public async Task Handle(IList<RecordPeriodEndSubmissionWindowValidationJob> messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                logger.LogInfo($"Handling period end submission window validation job: {message.ToJson()}");
                await periodEndJobService.RecordPeriodEndJob(message, cancellationToken);
                periodEndSubmissionWindowValidationJobStatusManager.StartMonitoringJob(message.JobId, JobType.PeriodEndSubmissionWindowValidationJob);
                logger.LogInfo($"Handled period end submission window validation job: {message.JobId}");
            }
        }
    }
}
