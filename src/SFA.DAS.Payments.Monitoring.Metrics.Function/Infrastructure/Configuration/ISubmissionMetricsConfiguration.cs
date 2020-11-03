namespace SFA.DAS.Payments.Monitoring.Metrics.Function.Infrastructure.Configuration
{
    public interface ISubmissionMetricsConfiguration
    {
        string PaymentsMetricsConnectionString { get; set; }
        string PaymentsConnectionString { get; set; }
    }
}