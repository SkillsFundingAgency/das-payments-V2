using System;
using System.Configuration;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure
{
    public class ApplicationConfiguration
    {
        public string AcceptanceTestsEndpointName => ConfigurationManager.AppSettings["EndpointName"];
        public string StorageConnectionString => ConfigurationManager.ConnectionStrings["StorageConnectionString"]?.ConnectionString ?? throw new InvalidOperationException("Failed to find the Storage connection string.");
        public string ServiceBusConnectionString => ConfigurationManager.ConnectionStrings["ServiceBusConnectionString"]?.ConnectionString ?? throw new InvalidOperationException("Failed to find the Service Bus Connection string.");
    }
}