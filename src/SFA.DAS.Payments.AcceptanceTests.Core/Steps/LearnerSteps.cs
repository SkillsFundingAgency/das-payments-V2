using System;
using SFA.DAS.Payments.AcceptanceTests.Core.Application;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Steps
{
    [Binding]
    public class LearnerSteps
    {
        private readonly LearnRefNumberGenerator learnRefNumberGenerator;
        private readonly ScenarioContext context;

        public LearnerSteps(LearnRefNumberGenerator generator, ScenarioContext context)
        {
            learnRefNumberGenerator = generator;
            this.context = context;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            context["SessionId"] = Guid.NewGuid();
        }

        [Given(@"a learner with LearnRefNumber (.*) and Uln (.*) undertaking training with training provider (.*)")]
        public void GivenALearnerWithLearnRefNumberAndUln(string learnRefNumber, long uln, long ukprn)
        {
            var learner = new Learner
            {
                Ukprn = ukprn,
                Uln = uln,
                LearnRefNumber = learnRefNumber,
                GeneratedLearnRefNumber = learnRefNumberGenerator.Generate(ukprn, learnRefNumber)
            };

            context.Set(learner);
        }

    }
}