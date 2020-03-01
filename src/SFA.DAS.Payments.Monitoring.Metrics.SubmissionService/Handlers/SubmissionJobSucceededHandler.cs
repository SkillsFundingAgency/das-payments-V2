using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.SubmissionService.Handlers
{
    public class SubmissionJobSucceededHandler : IHandleMessages<SubmissionJobSucceeded>
    {
        private readonly IPaymentLogger logger;
        private readonly ISubmissionMetricsService submissionMetricsService;
        private readonly IConfigurationHelper configurationHelper;

        public SubmissionJobSucceededHandler(IPaymentLogger logger, ISubmissionMetricsService submissionMetricsService,IConfigurationHelper configurationHelper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.submissionMetricsService = submissionMetricsService ?? throw new ArgumentNullException(nameof(submissionMetricsService));
            this.configurationHelper = configurationHelper ?? throw new ArgumentNullException(nameof(configurationHelper));
        }

        public async Task Handle(SubmissionJobSucceeded message, IMessageHandlerContext context)
        {
            if (!context.MessageHeaders.ContainsKey("Delayed"))
            {
                var delay = TimeSpan.Parse(
                    configurationHelper.GetSettingOrDefault("MetricsGenerationDelay", "00:05:00"));
                logger.LogInfo($"Delaying metrics generation for {delay.TotalSeconds} seconds for job: {message.JobId}");
                var options = new SendOptions();
                options.SetHeader("Delayed", DateTimeOffset.UtcNow.ToString("G")+delay);
                options.RouteToThisEndpoint();
                options.DelayDeliveryWith(delay);
                await context.Send(message, options).ConfigureAwait(false);
                context.DoNotContinueDispatchingCurrentMessageToHandlers();
                return;
            }

            logger.LogDebug($"Handling message to build metrics for: {message.ToJson()}");
            await submissionMetricsService.BuildMetrics(message.Ukprn, message.JobId, message.AcademicYear,
                message.CollectionPeriod, CancellationToken.None).ConfigureAwait(false);
            logger.LogInfo($"Finished handling the SubmissionJobSucceeded event: {message.ToJson()}");
        }
    }
}