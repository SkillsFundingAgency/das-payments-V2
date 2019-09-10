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

        public string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString ??
                   throw new InvalidOperationException($"{name} not found in connection strings.");
        }
    }
}