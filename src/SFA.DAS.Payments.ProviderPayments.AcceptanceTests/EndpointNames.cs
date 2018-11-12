using System.Configuration;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests
{
    public class EndpointNames
    {
        public static string AcceptanceTestEndpointName => ConfigurationManager.AppSettings["EndpointName"];
        public static string ProviderPaymentEndPointName => ConfigurationManager.AppSettings["ProviderPaymentsServiceEndpointName"];

    }
}