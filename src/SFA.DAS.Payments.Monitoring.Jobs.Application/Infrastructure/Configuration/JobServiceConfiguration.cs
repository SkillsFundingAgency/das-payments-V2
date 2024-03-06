using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration
{
    public interface IJobServiceConfiguration
    {
        TimeSpan JobStatusInterval { get; }
        TimeSpan EarningsJobTimeout { get; }
        TimeSpan PeriodEndRunJobTimeout { get; }
        TimeSpan PeriodEndStartJobTimeout { get; }
    }

    public class JobServiceConfiguration: IJobServiceConfiguration
    {
        public TimeSpan JobStatusInterval { get; }
        public TimeSpan EarningsJobTimeout { get; }
        public TimeSpan PeriodEndRunJobTimeout { get; }
        public TimeSpan PeriodEndStartJobTimeout { get; }

        public JobServiceConfiguration(TimeSpan jobStatusInterval, TimeSpan earningsJobTimeout, TimeSpan periodEndRunJobTimeout, TimeSpan periodEndStartJobTimeout)
        {
            JobStatusInterval = jobStatusInterval;
            EarningsJobTimeout = earningsJobTimeout;
            PeriodEndRunJobTimeout = periodEndRunJobTimeout;
            PeriodEndStartJobTimeout = periodEndStartJobTimeout;
        }
    }
}