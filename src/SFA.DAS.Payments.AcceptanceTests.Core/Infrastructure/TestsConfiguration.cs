using System;
using System.Configuration;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure
{
    public class TestsConfiguration
    {
        public string AcceptanceTestsEndpointName => GetAppSetting("EndpointName");
        public string StorageConnectionString => GetConnectionString("StorageConnectionString");
        public string ServiceBusConnectionString => GetConnectionString("ServiceBusConnectionString");
        public string PaymentsConnectionString => GetConnectionString("PaymentsConnectionString");
        public bool ValidateDcAndDasServices => bool.Parse(ConfigurationManager.AppSettings["ValidateDcAndDasServices"] ?? "false");
        public TimeSpan TimeToWait => TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToWait"] ?? "00:00:30");
        public TimeSpan TimeToWaitForUnexpected => TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToWaitForUnexpected"] ?? "00:00:30");
        public TimeSpan TimeToPause => TimeSpan.Parse(ConfigurationManager.AppSettings["TimeToPause"] ?? "00:00:05");
        public TimeSpan DefaultMessageTimeToLive => TimeSpan.Parse(ConfigurationManager.AppSettings["DefaultMessageTimeToLive"] ?? "00:20:00");
        public string GetAppSetting(string keyName) => ConfigurationManager.AppSettings[keyName] ?? throw new InvalidOperationException($"{keyName} not found in app settings.");
        public string GetConnectionString(string name) => ConfigurationManager.ConnectionStrings[name].ConnectionString ?? throw new InvalidOperationException($"{name} not found in connection strings.");
    }
}