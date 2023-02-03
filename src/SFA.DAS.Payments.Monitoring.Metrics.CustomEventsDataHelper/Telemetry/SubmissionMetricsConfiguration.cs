namespace SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper.Telemetry
{
    public interface ISubmissionMetricsConfiguration
    {
        string PaymentsConnectionString { get; set; }
    }

    public class SubmissionMetricsConfiguration : ISubmissionMetricsConfiguration
    {
        public string PaymentsConnectionString { get; set; }
    }
}