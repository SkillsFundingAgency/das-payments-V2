using System;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class LearnerSteps: RequiredPaymentsStepsBase
    {

        public LearnerSteps(ScenarioContext context): base(context)
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
    }
}