using System.Configuration;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests
{
    public class EndpointNames
    {
        public static string JobsService => ConfigurationManager.AppSettings["JobsServiceEndpointName"];
    }
}