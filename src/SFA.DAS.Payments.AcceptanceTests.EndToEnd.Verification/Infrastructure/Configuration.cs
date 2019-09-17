using System;
using System.Configuration;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
    public class Configuration
    {
        public string Ilr1819ContainerName => "ilr1819-files";

        public string Ilr1920ContainerName => "ilr1920-files";

        public string PaymentsConnectionString => GetConnectionString("PaymentsConnectionString");

        public string DcStorageConnectionString => GetConnectionString("DcStorageConnectionString");

        public TimeSpan SqlCommandTimeout => TimeSpan.Parse(GetAppSetting("SqlCommandTimeout") ?? "00:02:00");

        public TimeSpan MaxTimeout => TimeSpan.Parse(GetAppSetting("MaxTimeout") ?? "00:15:00");

        public TimeSpan DcJobEventCheckDelay => TimeSpan.Parse(GetAppSetting("DcJobEventCheckDelay") ?? "00:00:05");

        public bool ClearPaymentsData => bool.Parse(GetAppSetting("ValidateDcAndDasServices") ?? "false");

        public string GetAppSetting(string keyName)
        {
            return ConfigurationManager.AppSettings[keyName] ?? throw new InvalidOperationException($"{keyName} not found in app settings.");
        }

        public string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString ??
                   throw new InvalidOperationException($"{name} not found in connection strings.");
        }
    }
}