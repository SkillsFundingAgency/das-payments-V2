using System.Configuration;

namespace SFA.DAS.Payments.Audit.AcceptanceTests.Infrastructure
{
    public class EndpointNames
    {
        public static string AuditServiceEndpoint => ConfigurationManager.AppSettings["AuditFundingSourceServiceEndpointName"];
        public static string DataLockAuditServiceEndpoint => ConfigurationManager.AppSettings["AuditDataLockServiceEndpointName"];
    }
}