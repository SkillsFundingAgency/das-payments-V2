using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Infrastructure
{
    [Binding]
    public class FeatureScopedTestSession : TestSessionBase
    {
        public FeatureScopedTestSession(FeatureContext context) : base(context)
        {
        }

        [BeforeTestRun(Order = 2)]
        public static void AddDcConfig()
        {
            DcHelper.AddDcConfig(Builder);
        }

        [BeforeFeature(Order = 0)]
        public static void SetUpFeature(FeatureContext context)
        {
            SetUpTestSession(context);
        }

        [BeforeFeature(Order = 99)]
        public static void CleanUpFeature(FeatureContext context)
        {
            CleanUpTestSession(context);
        }
    }
}