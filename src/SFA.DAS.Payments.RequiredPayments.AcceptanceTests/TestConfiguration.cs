using System.Configuration;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests
{
    public class TestConfiguration
    {
        public static string StorageConnectionString =>
            ConfigurationManager.ConnectionStrings["StorageConnectionString"]?.ConnectionString;
    }
}