using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Infrastructure
{
    [Binding]
    public class FeatureScopedBootstrapper : BindingBootstrapper
    {
        public FeatureScopedBootstrapper(FeatureContext context) : base(context)
        {
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