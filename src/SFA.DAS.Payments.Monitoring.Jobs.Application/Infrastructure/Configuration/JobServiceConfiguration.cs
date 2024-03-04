using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration
{
    public interface IJobServiceConfiguration
    {
        TimeSpan JobStatusInterval { get; }
        TimeSpan EarningsJobTimeout { get; }
        TimeSpan PeriodEndRunJobTimeout { get; }
    }

    public class JobServiceConfiguration: IJobServiceConfiguration
    {
        public TimeSpan JobStatusInterval { get; }
        public TimeSpan EarningsJobTimeout { get; }
        public TimeSpan PeriodEndRunJobTimeout { get; }

        public JobServiceConfiguration(TimeSpan jobStatusInterval, TimeSpan earningsJobTimeout, TimeSpan periodEndRunJobTimeout)
        {
            JobStatusInterval = jobStatusInterval;
            EarningsJobTimeout = earningsJobTimeout;
            PeriodEndRunJobTimeout = periodEndRunJobTimeout;
        }
    }
}