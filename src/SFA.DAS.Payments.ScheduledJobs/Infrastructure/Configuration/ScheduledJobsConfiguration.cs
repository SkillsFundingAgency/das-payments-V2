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
        string PreviousAcademicYearCollectionPeriod { get; }
        string PreviousAcademicYear { get; }
        string EarningAuditDataCleanUpQueue { get; set; }
        string DataLockAuditDataCleanUpQueue { get; set; }
        string FundingSourceAuditDataCleanUpQueue { get; set; }
        string RequiredPaymentAuditDataCleanUpQueue { get; set; }
        int AccountApiBatchSize { get; set; }
    }

    public class ScheduledJobsConfiguration : IScheduledJobsConfiguration
    {
        public string EndpointName { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string DasNServiceBusLicenseKey { get; set; }
        public string LevyAccountBalanceEndpoint { get; set; }
        public string CurrentCollectionPeriod { get; set; }
        public string CurrentAcademicYear { get; set; }
        public string PreviousAcademicYearCollectionPeriod { get; set; }
        public string PreviousAcademicYear { get; set; }
        public string EarningAuditDataCleanUpQueue { get; set; }
        public string DataLockAuditDataCleanUpQueue { get; set; }
        public string FundingSourceAuditDataCleanUpQueue { get; set; }
        public string RequiredPaymentAuditDataCleanUpQueue { get; set; }
        public int AccountApiBatchSize { get; set; }
    }
}