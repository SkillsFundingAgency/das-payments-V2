using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Infrastructure.Messaging
{
    public class SubmissionMetricsGenerationTimeoutBehaviour : Behavior<IIncomingLogicalMessageContext>
    {
        private readonly ITelemetry telemetry;

        public SubmissionMetricsGenerationTimeoutBehaviour(ITelemetry telemetry)
        {
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            try
            {
                await next().ConfigureAwait(false);
            }
            catch (SubmissionMetricsTimeoutException timeoutException)
            {
                if (context.Message?.MessageType == typeof(GenerateSubmissionSummary))
                {
                    var message = context.Message.Instance as GenerateSubmissionSummary;
                    var errorMessage = $"Submission metrics generation timed out for UKPRN {message.Ukprn}";

                    var properties = new Dictionary<string, string>
                    {
                        {TelemetryKeys.CollectionPeriod, message.CollectionPeriod.ToString()},
                        {TelemetryKeys.AcademicYear, message.AcademicYear.ToString()},
                        {TelemetryKeys.JobId, message.JobId.ToString()},
                        {TelemetryKeys.Ukprn, message.Ukprn.ToString()}
                    };
                    
                    telemetry.TrackEvent(errorMessage, properties, new Dictionary<string, double>());
                }
            }
        }
    }
}
