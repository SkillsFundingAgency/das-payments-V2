﻿namespace SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.Configuration;

public class PeriodEndArchiveConfiguration : IPeriodEndArchiveConfiguration
{
    public string ResourceGroup { get; set; }
    public string AzureDataFactoryName { get; set; }
    public string PipeLine { get; set; }
    public string SubscriptionId { get; set; }
}