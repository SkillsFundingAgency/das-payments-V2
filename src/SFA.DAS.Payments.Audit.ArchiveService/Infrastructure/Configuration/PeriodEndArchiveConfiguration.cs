namespace SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.Configuration
{
    public class PeriodEndArchiveConfiguration : IPeriodEndArchiveConfiguration
    {
        public int SleepDelay { get; set; }
        public string TenantId { get; set; }
        public string ApplicationId { get; set; }
        public string AuthenticationKey { get; set; }
        public string ResourceGroup { get; set; }
        public string AzureDataFactoryName { get; set; }
        public string PipeLine { get; set; }
        public string AuthorityUri { get; set; }
        public string ManagementUri { get; set; }
        public string SubscriptionId { get; set; }
    }
}