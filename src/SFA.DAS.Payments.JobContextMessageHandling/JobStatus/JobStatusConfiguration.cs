using System;

namespace SFA.DAS.Payments.JobContextMessageHandling.JobStatus
{
    /// <summary>
    /// TODO: Temp solution to wait for jobs to finish
    /// </summary>
    [Obsolete("Temporary solution to wait for jobs to finish.  Should really use events from Job service but DC.JobContextManager doesn't support that kind of pattern")]
    public interface IJobStatusConfiguration
    {
        TimeSpan TimeToPauseBetweenChecks { get; }
        TimeSpan TimeToWaitForJobToComplete { get; }
        TimeSpan TimeToWaitForPeriodEndRunJobToComplete { get; }
    }

    /// <summary>
    /// TODO: Temp solution to wait for jobs to finish
    /// </summary>
    [Obsolete("Temporary solution to wait for jobs to finish.  Should really use events from Job service but DC.JobContextManager doesn't support that kind of pattern")]
    public class JobStatusConfiguration: IJobStatusConfiguration
    {
        public TimeSpan TimeToPauseBetweenChecks { get; set; }
        public TimeSpan TimeToWaitForJobToComplete { get; set; }
        public TimeSpan TimeToWaitForPeriodEndRunJobToComplete { get; set; }
    }
}