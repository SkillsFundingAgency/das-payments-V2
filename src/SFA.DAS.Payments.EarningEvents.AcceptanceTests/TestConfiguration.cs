using System.Configuration;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests
{
    public class TestConfiguration : DcConfiguration
    {
        public static string StorageConnectionString =>
            ConfigurationManager.ConnectionStrings["StorageConnectionString"]?.ConnectionString;
    }
}