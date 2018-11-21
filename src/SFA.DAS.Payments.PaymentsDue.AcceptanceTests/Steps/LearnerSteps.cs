using SFA.DAS.Payments.AcceptanceTests.Core;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class LearnerSteps : StepsBase
    {
        public LearnerSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"a learner is undertaking a training with a training provider")]
        public void GivenALearnerIsUndertakingATrainingWithATrainingProvider()
        {
            TestSession.Learners.Clear();
            TestSession.Learners.Add(TestSession.GenerateLearner());
        }

        [Given(@"following learners are undertaking training with a training provider")]
        public void GivenFollowingLearnersAreUndertakingTrainingWithATrainingProvider(Table table)
        {
            //TestSession.Learners.Clear();
            //foreach (var row in table.Rows)
            //{
            //    var learner = TestSession.GenerateLearner();
            //    learner.LearnRefNumber = TestSession.LearnRefNumberGenerator.Generate(learner.Ukprn, row["LearnerId"]);
            //    TestSession.Learners.Add(learner);
            //}
        }
    }
}