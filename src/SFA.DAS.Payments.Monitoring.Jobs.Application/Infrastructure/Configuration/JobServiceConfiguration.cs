using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration
{
    public interface IJobServiceConfiguration
    {
        TimeSpan JobStatusInterval { get; }
        TimeSpan EarningsJobTimeout { get; }
    }

    public class JobServiceConfiguration: IJobServiceConfiguration
    {
        public TimeSpan JobStatusInterval { get; }
        public TimeSpan EarningsJobTimeout { get; }

        public JobServiceConfiguration(TimeSpan jobStatusInterval, TimeSpan earningsJobTimeout)
        {
            JobStatusInterval = jobStatusInterval;
            EarningsJobTimeout = earningsJobTimeout;
        }
    }
}