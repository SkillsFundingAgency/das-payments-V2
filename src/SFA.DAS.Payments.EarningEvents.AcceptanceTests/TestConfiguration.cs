using System.Configuration;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests
{
    public class TestConfiguration
    {
        public static string StorageConnectionString =>
            ConfigurationManager.ConnectionStrings["StorageConnectionString"]?.ConnectionString;

        public static string ServiceBusConnectionString =>
            ConfigurationManager.ConnectionStrings["ServiceBusConnectionString"]?.ConnectionString;

        public static string ServiceBusQueue =>
            ConfigurationManager.AppSettings["ServiceBusQueue"];

        public static string AzureRedisConnectionString =>
            ConfigurationManager.ConnectionStrings["AzureRedisConnectionString"]?.ConnectionString;
    }
}