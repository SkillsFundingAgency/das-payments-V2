using NServiceBus;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Infrastructure.Messaging
{
    public class SubmissionMetricsMessaagingRegistration : INeedInitialization
    {
        public void Customize(EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.Pipeline.Register(typeof(SubmissionMetricsGenerationFailureBehaviour), "Logs telemetry on submission metrics generation failure");
        }
    }
}
