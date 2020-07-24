using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Metrics.SubmissionService.Handlers
{
    public class SubmissionJobSucceededHandler : IHandleMessages<SubmissionJobSucceeded>
    {
        private readonly IPaymentLogger logger;
        private readonly ISubmissionMetricsService submissionMetricsService;
        private readonly IConfigurationHelper configurationHelper;

        public SubmissionJobSucceededHandler(IPaymentLogger logger, ISubmissionMetricsService submissionMetricsService, IConfigurationHelper configurationHelper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.submissionMetricsService = submissionMetricsService ?? throw new ArgumentNullException(nameof(submissionMetricsService));
            this.configurationHelper = configurationHelper ?? throw new ArgumentNullException(nameof(configurationHelper));
        }

        public async Task Handle(SubmissionJobSucceeded message, IMessageHandlerContext context)
        {
            var delay = TimeSpan.Parse(
                configurationHelper.GetSettingOrDefault("MetricsGenerationDelay", "00:05:00"));
            logger.LogDebug($"Delaying metrics generation for {delay.TotalSeconds} seconds for job: {message.JobId}");
            var options = new SendOptions();
            options.RouteToThisEndpoint();
            options.DelayDeliveryWith(delay);
            await context.Send(new GenerateSubmissionSummary
            {
                CollectionPeriod = message.
                    CollectionPeriod, 
                JobId = message.JobId, 
                Ukprn = message.Ukprn, 
                AcademicYear = message.AcademicYear
            }, options).ConfigureAwait(false);
            context.DoNotContinueDispatchingCurrentMessageToHandlers();
            logger.LogInfo($"Delayed metrics generation for {delay.TotalSeconds} seconds for job: {message.JobId}");
        }
    }
}