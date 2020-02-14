using System;

namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration
{
    public interface IScheduledJobsConfiguration
    {
        string EndpointName { get; }
        string ServiceBusConnectionString { get; }
        string NServiceBusLicense { get; }
        string LevyAccountBalanceEndpoint { get; }
    }
}