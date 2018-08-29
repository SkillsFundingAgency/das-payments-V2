using System;
using System.Linq;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Application;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class LearnerSteps
    {
        private readonly ScenarioContext scenarioContext;

        private readonly LearnRefNumberGenerator learnRefNumberGenerator;

        public LearnerSteps(ScenarioContext context, LearnRefNumberGenerator generator)
        {
            scenarioContext = context;
            learnRefNumberGenerator = generator;
        }

        [Given(@"the following learners:")]
        public void GivenTheFollowingLearners(Table table)
        {
            var learners = table.CreateSet<Learner>().ToList();

            learners.ForEach(l =>
            {
                l.GeneratedLearnRefNumber = learnRefNumberGenerator.Generate(l.Ukprn, l.LearnRefNumber).ToString();

                
            });

            scenarioContext["Learners"] = learners;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            scenarioContext["SessionId"] = Guid.NewGuid();
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