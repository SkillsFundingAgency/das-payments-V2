using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration
{
    public interface IJobServiceConfiguration
    {
        TimeSpan JobStatusInterval { get; }
        TimeSpan EarningsJobTimeout { get; }
        TimeSpan PeriodEndRunJobTimeout { get; }
        TimeSpan PeriodEndStartJobTimeout { get; }
        TimeSpan TimeToWaitToReceivePeriodEndILRSubmissions { get; }
        TimeSpan TimeToWaitForPeriodEndArchiveJobToComplete { get; }
    }

    public class JobServiceConfiguration: IJobServiceConfiguration
    {
        public TimeSpan JobStatusInterval { get; }
        public TimeSpan EarningsJobTimeout { get; }
        public TimeSpan PeriodEndRunJobTimeout { get; }
        public TimeSpan PeriodEndStartJobTimeout { get; }
        public TimeSpan TimeToWaitToReceivePeriodEndILRSubmissions { get; }
        public TimeSpan TimeToWaitForPeriodEndArchiveJobToComplete { get; }

        public JobServiceConfiguration(TimeSpan jobStatusInterval, TimeSpan earningsJobTimeout, 
                                       TimeSpan periodEndRunJobTimeout, TimeSpan timeToWaitToReceivePeriodEndILRSubmissions, 
                                       TimeSpan periodEndStartJobTimeout, TimeSpan timeToWaitForPeriodEndArchiveJobToComplete)
        {
            JobStatusInterval = jobStatusInterval;
            EarningsJobTimeout = earningsJobTimeout;
            PeriodEndRunJobTimeout = periodEndRunJobTimeout;
            PeriodEndStartJobTimeout = periodEndStartJobTimeout;
            TimeToWaitToReceivePeriodEndILRSubmissions = timeToWaitToReceivePeriodEndILRSubmissions;
            TimeToWaitForPeriodEndArchiveJobToComplete = timeToWaitForPeriodEndArchiveJobToComplete;
        }
    }
}