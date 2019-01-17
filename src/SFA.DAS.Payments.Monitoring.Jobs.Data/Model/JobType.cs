namespace SFA.DAS.Payments.Monitoring.Jobs.Data.Model
{
    public enum JobType : byte
    {
        EarningsJob = 1,
        MonthEndJob, 
        ComponentAcceptanceTestEarningsJob,
        ComponentAcceptanceTestMonthEndJob
    }
}