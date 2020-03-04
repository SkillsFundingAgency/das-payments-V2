using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.SubmissionService.Handlers
{
    public class SubmissionJobSucceededHandler : IHandleMessages<SubmissionJobSucceeded>
    {
        private readonly IPaymentLogger logger;
        private readonly ISubmissionMetricsService submissionMetricsService;

        public SubmissionJobSucceededHandler(IPaymentLogger logger, ISubmissionMetricsService submissionMetricsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.submissionMetricsService = submissionMetricsService ?? throw new ArgumentNullException(nameof(submissionMetricsService));
        }

        public async Task Handle(SubmissionJobSucceeded message, IMessageHandlerContext context)
        {
            logger.LogDebug($"Handling message to build metrics for: {message.ToJson()}");
            await submissionMetricsService.BuildMetrics(message.Ukprn, message.JobId, message.AcademicYear,
                message.CollectionPeriod, CancellationToken.None).ConfigureAwait(false);
            logger.LogInfo($"Finished handling the SubmissionJobSucceeded event: {message.ToJson()}");
        }
    }
}