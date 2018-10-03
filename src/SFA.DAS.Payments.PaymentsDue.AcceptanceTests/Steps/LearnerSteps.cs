using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class LearnerSteps
    {
        private LearnRefNumberGenerator learnRefNumberGenerator;

        public LearnerSteps(ScenarioContext context)
        {
            this.learnRefNumberGenerator = new LearnRefNumberGenerator(context);
        }

        //[BeforeScenario]
        //public void BeforeScenario()
        //{

        //}

        [Given(@"a learner is undertaking a training with a training provider")]
        public void GivenALearnerIsUndertakingATrainingWithATrainingProvider()
        {
        }
    }
}