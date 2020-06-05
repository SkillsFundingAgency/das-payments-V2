namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration
{
    public interface IScheduledJobsConfiguration
    {
        string EndpointName { get; }
        string ServiceBusConnectionString { get; }
        string DasNServiceBusLicenseKey { get; }
        string LevyAccountBalanceEndpoint { get; }
        string CollectionPeriod { get; }
        string AcademicYear { get; }
        string EarningAuditDataCleanUpQueue { get; set; }
        string DataLockAuditDataCleanUpQueue { get; set; }
        string FundingSourceAuditDataCleanUpQueue { get; set; }
        string RequiredPaymentAuditDataCleanUpQueue { get; set; }
    }
}