using System;
using System.Configuration;

namespace SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Infrastructure
{
    public class TestConfiguration
    {
        public static Uri AuditDataCleanupFunctionUrl => new Uri(ConfigurationManager.AppSettings["AuditDataCleanupFunctionUrl"], UriKind.Absolute);
        
        public string PaymentsConnectionString => GetConnectionString("PaymentsConnectionString");
        public string GetConnectionString(string name) => ConfigurationManager.ConnectionStrings[name].ConnectionString ?? throw new InvalidOperationException($"{name} not found in connection strings.");
        public short CurrentAcademicYear => short.Parse(ConfigurationManager.AppSettings["CurrentAcademicYear"]);
        public short PreviousAcademicYear => short.Parse(ConfigurationManager.AppSettings["PreviousAcademicYear"]);
    }
}