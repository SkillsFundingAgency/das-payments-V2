﻿namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration
{
    public class ScheduledJobsConfiguration : IScheduledJobsConfiguration
    {
        public string EndpointName { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string DasNServiceBusLicenseKey { get; set; }
        public string LevyAccountBalanceEndpoint { get; set; }
        public string CollectionPeriod { get; set; }
        public string AcademicYear { get; set; }
        public string EarningAuditDataCleanUpQueue { get; set; }
        public string DataLockAuditDataCleanUpQueue { get; set; }
        public string FundingSourceAuditDataCleanUpQueue { get; set; }
        public string RequiredPaymentAuditDataCleanUpQueue { get; set; }
    }
}