using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure
{
    public class FeatureScopedBootstrapper : BindingBootstrapper
    {
        public FeatureScopedBootstrapper(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [BeforeFeature(Order = 0)]
        protected override void SetUpTestSession()
        {
            base.SetUpTestSession();
        }

        [AfterFeature(Order = 99)]
        protected override void CleanUpTestSession()
        {
            base.CleanUpTestSession();
        }
    }
}