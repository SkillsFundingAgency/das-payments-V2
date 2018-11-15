using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Components.Core.Infrastructure
{
    [Binding]
    public class ScenarioScopedTestSession : TestSessionBase
    {
        public ScenarioScopedTestSession(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [BeforeScenario(Order = 0)]
        public void SetUpTestSession()
        {
            SetUpTestSession(Context);
        }

        [AfterScenario(Order = 99)]
        public void CleanUpTestSession()
        {
            CleanUpTestSession(Context);
        }
    }
}
