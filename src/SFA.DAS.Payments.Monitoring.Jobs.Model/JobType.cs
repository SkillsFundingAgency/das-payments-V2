namespace SFA.DAS.Payments.Monitoring.Jobs.Model
{
    public enum JobType : byte
    {
        EarningsJob = 1,
        PeriodEndStartJob, 
        ComponentAcceptanceTestEarningsJob,
        ComponentAcceptanceTestMonthEndJob,
        PeriodEndRunJob,
        PeriodEndStopJob,
        PeriodEndSubmissionWindowValidationJob
    }
}