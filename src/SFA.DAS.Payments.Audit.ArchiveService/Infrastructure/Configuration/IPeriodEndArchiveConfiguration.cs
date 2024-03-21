namespace SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.Configuration
{
    public interface IPeriodEndArchiveConfiguration
    {
        string ResourceGroup { get; set; }
        string AzureDataFactoryName { get; set; }
        string PipeLine { get; set; }
        string SubscriptionId { get; set; }
        string TenantId { get; set; }
        string ApplicationId { get; set; }
        string AuthenticationKey { get; set; }
        string AuthorityUri { get; set; }
        string ManagementUri { get; set; }
        int SleepDelay { get; set; }
    }
}