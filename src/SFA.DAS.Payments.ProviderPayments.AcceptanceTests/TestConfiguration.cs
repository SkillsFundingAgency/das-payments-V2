using System.Configuration;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests
{
    public class TestConfiguration
    {
        public static string StorageConnectionString =>ConfigurationManager.ConnectionStrings["StorageConnectionString"]?.ConnectionString;

        public static string ServiceBusConnectionString => ConfigurationManager.ConnectionStrings["ServiceBusConnectionString"]?.ConnectionString;
        
        public static string DasServiceBusConnectionString => ConfigurationManager.ConnectionStrings["DASServiceBusConnectionString"]?.ConnectionString;
    }
}