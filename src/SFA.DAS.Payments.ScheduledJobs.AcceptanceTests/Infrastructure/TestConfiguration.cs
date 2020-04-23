using System;
using System.Configuration;

namespace SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Infrastructure
{
    public class TestConfiguration
    {
        public static Uri EarningFunctionUrl => new Uri(ConfigurationManager.AppSettings["EarningFunctionUrl"], UriKind.Absolute);
        public static Uri FundingSourceFunctionUrl => new Uri(ConfigurationManager.AppSettings["FundingSourceFunctionUrl"], UriKind.Absolute);
        public static Uri RequiredPaymentFunctionUrl => new Uri(ConfigurationManager.AppSettings["RequiredPaymentFunctionUrl"], UriKind.Absolute);
        public static Uri DataLockEvenFunctionUrl => new Uri(ConfigurationManager.AppSettings["DataLockEvenFunctionUrl"], UriKind.Absolute);
        
        public string PaymentsConnectionString => GetConnectionString("PaymentsConnectionString");
        public string GetConnectionString(string name) => ConfigurationManager.ConnectionStrings[name].ConnectionString ?? throw new InvalidOperationException($"{name} not found in connection strings.");
    }
}