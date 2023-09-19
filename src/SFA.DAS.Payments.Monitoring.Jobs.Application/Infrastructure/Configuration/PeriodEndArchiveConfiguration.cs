namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration
{
    public interface IPeriodEndArchiveConfiguration
    {
        string ArchiveFunctionUrl { get; }
        int ArchiveTimeout { get; }
        string ArchiveApiKey { get; }
    }

    public class PeriodEndArchiveConfiguration : IPeriodEndArchiveConfiguration
    {
        public PeriodEndArchiveConfiguration(string archiveFunctionUrl, int archiveTimeout, string archiveApiKey)
        {
            ArchiveFunctionUrl = archiveFunctionUrl;
            ArchiveTimeout = archiveTimeout;
            ArchiveApiKey = archiveApiKey;
        }

        public string ArchiveFunctionUrl { get; }
        public int ArchiveTimeout { get; }
        public string ArchiveApiKey { get; }
    }
}