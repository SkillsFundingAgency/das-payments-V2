using System;
using System.Linq;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Application;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class LearnerSteps
    {
        private readonly LearnRefNumberGenerator learnRefNumberGenerator;

        public LearnerSteps(LearnRefNumberGenerator generator)
        {
            learnRefNumberGenerator = generator;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            ScenarioContext.Current["SessionId"] = Guid.NewGuid();
        }

        [Given(@"a learner with LearnRefNumber (.*) and Uln (.*) undertaking training with training provider (.*)")]
        public void GivenALearnerWithLearnRefNumberAndUln(string learnRefNumber, long uln, long ukprn)
        {
            ScenarioContext.Current["LearnRefNumber"] = learnRefNumber;
            ScenarioContext.Current["Uln"] = uln;
            ScenarioContext.Current["Ukprn"] = ukprn;

            ScenarioContext.Current["GeneratedLearnRefNumber"] =
                learnRefNumberGenerator.Generate(ukprn, learnRefNumber);
        }

    }
}