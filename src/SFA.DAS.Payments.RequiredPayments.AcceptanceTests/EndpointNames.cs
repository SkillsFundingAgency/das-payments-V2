using System.Configuration;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests
{
    public class EndpointNames
    {
        public static string RequiredPayments => ConfigurationManager.AppSettings["RequiredPaymentsServiceEndpointName"];
    }
}