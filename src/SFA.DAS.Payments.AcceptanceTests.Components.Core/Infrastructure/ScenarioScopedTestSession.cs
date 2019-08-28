using System.Linq;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Components.Core.Infrastructure
{
    [Binding]
    public class ScenarioScopedTestSession : TestSessionBase
    {
        public ScenarioScopedTestSession(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [BeforeTestRun(Order = 45)]
        public static void CreateJobsDataContext()
        {
            Builder.Register(c =>
            {
                var configHelper = c.Resolve<TestsConfiguration>();
                var dataContext = new JobsDataContext(configHelper.PaymentsConnectionString);
                return dataContext;
            }).InstancePerLifetimeScope();
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
