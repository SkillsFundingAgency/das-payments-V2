using System.Configuration;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests
{
    public class EndpointNames
    {
        public static string PaymentsDue => ConfigurationManager.AppSettings["PaymentsDueServiceEndpointName"];
    }
}