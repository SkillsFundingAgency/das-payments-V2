using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;

namespace SFA.DAS.Payments.AcceptanceTests.Services.Configuration
{
    public class AzureStorageServiceConfig : IAzureStorageKeyValuePersistenceServiceConfig
    {
        public string ConnectionString => ConfigurationManager.ConnectionStrings["DcStorageConnectionString"]?.ConnectionString;

        public string ContainerName => ConfigurationManager.AppSettings["DcBlobStorageContainer"];
    }
}
