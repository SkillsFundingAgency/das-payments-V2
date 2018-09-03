using System.Configuration;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests
{
    public class TestConfiguration
    {
        public static string StorageConnectionString =>ConfigurationManager.ConnectionStrings["StorageConnectionString"]?.ConnectionString;
    }
}