using ESFA.DC.IO.AzureStorage.Config.Interfaces;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class AzureStorageKeyValuePersistenceConfig : IAzureStorageKeyValuePersistenceServiceConfig
    {
        public AzureStorageKeyValuePersistenceConfig(string connectionString, string containerName)
        {
            ConnectionString = connectionString;
            ContainerName = containerName;
        }

        public string ConnectionString { get; }

        public string ContainerName { get; }
    }
}