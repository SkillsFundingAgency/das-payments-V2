using System.Configuration;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests
{
    public class TestConfiguration
    {
        public static string StorageConnectionString =>
            ConfigurationManager.ConnectionStrings["StorageConnectionString"]?.ConnectionString;

        public static string DcServiceBusConnectionString =>
            ConfigurationManager.ConnectionStrings["DCServiceBusConnectionString"]?.ConnectionString;

        public static string ServiceBusQueue =>
            ConfigurationManager.AppSettings["ServiceBusQueue"];

        public static string AzureRedisConnectionString =>
            ConfigurationManager.ConnectionStrings["AzureRedisConnectionString"]?.ConnectionString;

        public static string TopicName => ConfigurationManager.AppSettings["TopicName"];

        public static string SubscriptionName => ConfigurationManager.AppSettings["SubscriptionName"];
    }
}