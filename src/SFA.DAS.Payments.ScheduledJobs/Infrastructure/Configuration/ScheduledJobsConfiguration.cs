namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration
{
    public class ScheduledJobsConfiguration : IScheduledJobsConfiguration
    {
        public string EndpointName { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string DasNServiceBusLicenseKey { get; set; }
        public string LevyAccountBalanceEndpoint { get; set; }
    }
}