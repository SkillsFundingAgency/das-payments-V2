using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class LearnerSteps : RequiredPaymentsStepsBase
    {

        public LearnerSteps(ScenarioContext context) : base(context)
        {
        }

        [BeforeScenario]
        public void BeforeScenario()
        {

        }

        [Given(@"a learner is undertaking a training with a training provider")]
        public void GivenALearnerIsUndertakingATrainingWithATrainingProvider()
        {
            // TODO: redundant, remove this
        }

        [Given(@"a learner with LearnRefNumber (.*) and Uln (.*) undertaking training with training provider (.*)")]
        public void GivenALearnerWithLearnRefNumberAndUln(string learnRefNumber, long uln, long ukprn)
        {
            //TODO: TestSession now contains the learner, urkprn, etc. get rid of those ref numbers, etc
        }

        [Given(@"following learners are undertaking training with a training provider")]
        public void GivenFollowingLearnersAreUndertakingTrainingWithATrainingProvider(Table table)
        {
            TestSession.Learners.Clear();
            foreach (var row in table.Rows)
            {
                var learner = TestSession.GenerateLearner();
                learner.LearnRefNumber = TestSession.LearnRefNumberGenerator.Generate(learner.Ukprn, row["LearnerId"]);
                TestSession.Learners.Add(learner);
            }
        }
    }
}