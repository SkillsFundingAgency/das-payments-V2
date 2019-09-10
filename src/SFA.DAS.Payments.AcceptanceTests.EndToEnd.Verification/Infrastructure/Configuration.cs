using System;
using System.Configuration;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
    public class Configuration
    {
        public string PaymentsConnectionString => GetConnectionString("PaymentsConnectionString");
        public string GetConnectionString(string name) => ConfigurationManager.ConnectionStrings[name].ConnectionString ?? throw new InvalidOperationException($"{name} not found in connection strings.");
    }
}
