using System.Configuration;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests
{
    public class EndpointNames
    {
        public static string EarningEventsService  => ConfigurationManager.AppSettings["EarningEventsServiceEndpointName"]; 
    }
}