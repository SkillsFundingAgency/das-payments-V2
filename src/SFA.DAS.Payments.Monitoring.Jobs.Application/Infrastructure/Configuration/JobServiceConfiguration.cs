using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration
{
    public interface IJobServiceConfiguration
    {
        TimeSpan JobStatusInterval { get; }
        TimeSpan EarningsJobTimeout { get; }
        TimeSpan PeriodEndRunJobTimeout { get; }
        TimeSpan TimeToWaitToReceivePeriodEndILRSubmissions { get; }
    }

    public class JobServiceConfiguration: IJobServiceConfiguration
    {
        public TimeSpan JobStatusInterval { get; }
        public TimeSpan EarningsJobTimeout { get; }
        public TimeSpan PeriodEndRunJobTimeout { get; }
        public TimeSpan TimeToWaitToReceivePeriodEndILRSubmissions { get; }

        public JobServiceConfiguration(TimeSpan jobStatusInterval, TimeSpan earningsJobTimeout, TimeSpan periodEndRunJobTimeout, TimeSpan timeToWaitToReceivePeriodEndILRSubmissions)
        {
            JobStatusInterval = jobStatusInterval;
            EarningsJobTimeout = earningsJobTimeout;
            PeriodEndRunJobTimeout = periodEndRunJobTimeout;
            TimeToWaitToReceivePeriodEndILRSubmissions = timeToWaitToReceivePeriodEndILRSubmissions;
        }
    }
}