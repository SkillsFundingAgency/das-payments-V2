using System;
using System.Configuration;

namespace SFA.DAS.Payments.Monitoring.Metrics.Function.AcceptanceTests.Infrastructure
{
    public class TestConfiguration
    {
        public Uri ValidateSubmissionWindowFunctionUrl => new Uri(ConfigurationManager.AppSettings["ValidateSubmissionWindowFunctionUrl"], UriKind.Absolute);
        
        public string PaymentsConnectionString => GetConnectionString("PaymentsConnectionString");
        public string GetConnectionString(string name) => ConfigurationManager.ConnectionStrings[name].ConnectionString ?? throw new InvalidOperationException($"{name} not found in connection strings.");
    }
}
