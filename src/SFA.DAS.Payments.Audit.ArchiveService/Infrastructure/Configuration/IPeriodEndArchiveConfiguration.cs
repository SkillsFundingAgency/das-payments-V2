namespace SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.Configuration;

public interface IPeriodEndArchiveConfiguration
{
    string ResourceGroup { get; set; }
    string AzureDataFactoryName { get; set; }
    string PipeLine { get; set; }
    string SubscriptionId { get; set; }
}