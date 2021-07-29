namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration
{
    public interface IScheduledJobsConfiguration
    {
        string EndpointName { get; }
        string ServiceBusConnectionString { get; }
        string DasNServiceBusLicenseKey { get; }
        string LevyAccountBalanceEndpoint { get; }
        string CurrentCollectionPeriod { get; }
        string CurrentAcademicYear { get; }
        string PreviousCollectionPeriod { get; }
        string PreviousAcademicYear { get; }
        string EarningAuditDataCleanUpQueue { get; set; }
        string DataLockAuditDataCleanUpQueue { get; set; }
        string FundingSourceAuditDataCleanUpQueue { get; set; }
        string RequiredPaymentAuditDataCleanUpQueue { get; set; }
        int AccountApiBatchSize { get; set; }
    }
}