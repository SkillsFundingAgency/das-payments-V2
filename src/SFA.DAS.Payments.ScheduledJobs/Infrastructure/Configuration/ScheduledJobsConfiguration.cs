namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration
{
    public class ScheduledJobsConfiguration : IScheduledJobsConfiguration
    {
        public string EndpointName { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string NServiceBusLicense { get; set; }
    }
}