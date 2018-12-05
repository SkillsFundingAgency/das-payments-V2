using System.Configuration;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class DcConfiguration
    {
        public static string DcServiceBusConnectionString =>
            ConfigurationManager.ConnectionStrings["DCServiceBusConnectionString"]?.ConnectionString;

        public static string AzureTableStorageConnectionString =>
            ConfigurationManager.ConnectionStrings["AzureTableStorageConnectionString"]?.ConnectionString;

        public static string DcTableStorageContainer => ConfigurationManager.AppSettings["DcTableStorageContainer"];

        public static string TopicName => ConfigurationManager.AppSettings["TopicName"];

        public static string SubscriptionName => ConfigurationManager.AppSettings["SubscriptionName"];

        public static string ServiceBusQueue =>
            ConfigurationManager.AppSettings["ServiceBusQueue"];
    }
}
