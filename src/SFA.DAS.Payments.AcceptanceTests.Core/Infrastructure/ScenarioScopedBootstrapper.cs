using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure
{
    public class ScenarioScopedBootstrapper : BindingBootstrapper
    {
        public ScenarioScopedBootstrapper(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [BeforeScenario(Order = 0)]
        protected override void SetUpTestSession()
        {
            base.SetUpTestSession();
        }

        [AfterScenario(Order = 99)]
        protected override void CleanUpTestSession()
        {
            base.CleanUpTestSession();
        }
    }
}
