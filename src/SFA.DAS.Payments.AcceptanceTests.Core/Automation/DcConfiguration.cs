using System.Configuration;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class DcConfiguration
    {
        public static string DcServiceBusConnectionString => ConfigurationManager.ConnectionStrings["DCServiceBusConnectionString"]?.ConnectionString;

        public static string DcStorageConnectionString => ConfigurationManager.ConnectionStrings["DcStorageConnectionString"]?.ConnectionString;

        public static string DcBlobStorageContainer => ConfigurationManager.AppSettings["DcBlobStorageContainer"];

        public static string PeriodEndTopicName => ConfigurationManager.AppSettings["PeriodEnd_TopicName"];
        public static string TopicName => ConfigurationManager.AppSettings["TopicName"];

        public static string SubscriptionName => ConfigurationManager.AppSettings["SubscriptionName"];
        public static string PeriodEndSubscriptionName => ConfigurationManager.AppSettings["PeriodEnd_SubscriptionName"];

        public static string ServiceBusQueue => ConfigurationManager.AppSettings["ServiceBusQueue"];
    }
}
