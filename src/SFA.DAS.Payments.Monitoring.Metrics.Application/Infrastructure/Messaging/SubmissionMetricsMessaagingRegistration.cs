using NServiceBus;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Infrastructure.Messaging
{
    public class SubmissionMetricsMessaagingRegistration : INeedInitialization
    {
        public void Customize(EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.Pipeline.Register(typeof(SubmissionMetricsGenerationTimeoutBehaviour), "Logs telemetry when submission metrics generation times out");
        }
    }
}
