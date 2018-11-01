using System.Linq;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure
{
    public class ScenarioScopedBootstrapper : BindingBootstrapper
    {
        public ScenarioScopedBootstrapper(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [BeforeScenario(Order = 0)]
        public void SetUpTestSession()
        {
            if (ScenarioContext.Current.ScenarioInfo.Tags.Contains("EndToEnd"))
                return;
            SetUpTestSession(Context);
        }

        [AfterScenario(Order = 99)]
        public void CleanUpTestSession()
        {
            if (ScenarioContext.Current.ScenarioInfo.Tags.Contains("EndToEnd"))
                return;
            CleanUpTestSession(Context);
        }
    }
}
