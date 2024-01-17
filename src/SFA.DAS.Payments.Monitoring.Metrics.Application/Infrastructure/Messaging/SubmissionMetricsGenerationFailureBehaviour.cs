using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Infrastructure.Messaging
{
    public class SubmissionMetricsGenerationFailureBehaviour : Behavior<IIncomingLogicalMessageContext>
    {
        private readonly ITelemetry telemetry;

        public SubmissionMetricsGenerationFailureBehaviour(ITelemetry telemetry)
        {
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            try
            {
                await next().ConfigureAwait(false);
            }
            catch (SubmissionMetricsGenerationException timeoutException)
            {
                if (context.Message?.MessageType == typeof(GenerateSubmissionSummary))
                {
                    var message = context.Message.Instance as GenerateSubmissionSummary;
                    var errorMessage = $"Submission metrics generation failed for Provider: {message.Ukprn}";

                    var properties = new Dictionary<string, string>
                    {
                        {TelemetryKeys.CollectionPeriod, message.CollectionPeriod.ToString()},
                        {TelemetryKeys.AcademicYear, message.AcademicYear.ToString()},
                        {TelemetryKeys.JobId, message.JobId.ToString()},
                        {TelemetryKeys.Ukprn, message.Ukprn.ToString()}
                    };

                    var metrics = new Dictionary<string, double>
                    {
                        {TelemetryKeys.InternalJobId, message.JobId}
                    };
                    
                    telemetry.TrackEvent(errorMessage, properties, metrics);
                }
            }
        }
    }
}
