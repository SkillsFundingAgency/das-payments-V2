namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration
{
    public interface IScheduledJobsConfiguration
    {
        string EndpointName { get; }
        string ServiceBusConnectionString { get; }
        string DasNServiceBusLicenseKey { get; }
        string LevyAccountBalanceEndpoint { get; }
    }
}