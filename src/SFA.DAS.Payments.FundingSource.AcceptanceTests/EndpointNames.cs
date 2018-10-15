using System.Configuration;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests
{
    public class EndpointNames
    {
        public static string NonLevyFundedPaymentsService => ConfigurationManager.AppSettings["NonLevyFundedPaymentsServiceEndpointName"];
    }
}