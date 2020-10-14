namespace SFA.DAS.Payments.Monitoring.Metrics.Function.Infrastructure.Configuration
{
    public class SubmissionMetricsConfiguration : ISubmissionMetricsConfiguration
    {
        public string PaymentsMetricsConnectionString { get; set; }
        public string PaymentsConnectionString { get; set; }
    }
}