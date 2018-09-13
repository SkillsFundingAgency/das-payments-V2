using System;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Application;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
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
            context["LearnRefNumber"] = learnRefNumber;
            context["Uln"] = uln;
            context["Ukprn"] = ukprn;

            context["GeneratedLearnRefNumber"] =
                learnRefNumberGenerator.Generate(ukprn, learnRefNumber);
        }

    }
}