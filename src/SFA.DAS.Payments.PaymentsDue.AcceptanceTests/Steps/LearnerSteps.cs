using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class LearnerSteps : StepsBase
    {
        private LearnRefNumberGenerator learnRefNumberGenerator;

        public LearnerSteps(ScenarioContext context) : base(context)
        {
            this.learnRefNumberGenerator = new LearnRefNumberGenerator(context);
        }

        [BeforeScenario]
        public void BeforeScenario()
        {

        }

        [Given(@"a learner with LearnRefNumber learnref(.*) and Uln (.*) undertaking training with training provider (.*)")]
        public void GivenALearnerWithLearnRefNumberAndUln(int learnRefNumber, long uln, long ukprn)
        {
            //TestSession.Learner.LearnRefNumber = learnRefNumberGenerator.Generate(ukprn, learnRefNumber.ToString());
            //TestSession.Ukprn = ukprn;
            //TestSession.Learner.Uln = do I care?
        }
    }
}