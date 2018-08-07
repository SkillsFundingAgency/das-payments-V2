using SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class LearnerSteps
    {
        private readonly ScenarioContext scenarioContext;

        public LearnerSteps(ScenarioContext context)
        {
            scenarioContext = context;
        }

        [Given(@"the following learners:")]
        public void GivenTheFollowingLearners(Table table)
        {
            var learners = table.CreateSet<Learner>();

            scenarioContext["Learners"] = learners;
        }
    }
}