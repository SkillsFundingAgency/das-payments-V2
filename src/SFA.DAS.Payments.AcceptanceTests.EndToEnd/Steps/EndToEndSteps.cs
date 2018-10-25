using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class EndToEndSteps : StepsBase
    {
        public EndToEndSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"the provider is providing trainging for the following learners")]
        public void GivenTheProviderIsProvidingTraingingForTheFollowingLearners(Table table)
        {
            var trainings = table.CreateSet<Training>().ToList();
            SfaContributionPercentage = trainings[0].SfaContributionPercentage;
        }


        [When(@"the ILR file is submitted for the learners for collection period R(.*)/Current Academic Year")]
        public void WhenTheILRFileIsSubmittedForTheLearnersForCollectionPeriodRCurrentAcademicYear(int period)
        {
            SetCurrentCollectionYear();
            CollectionPeriod = (byte) period;
        }

        [Then(@"the following learner earnings should be generated")]
        public void ThenTheFollowingLearnerEarningsShouldBeGenerated(Table table)
        {
            var expectedEarnings = table.CreateSet<Earning>().ToList();
            WaitForIt(() => EarningEventMatcher.MatchEarnings(expectedEarnings, TestSession.Ukprn), "Earning event check failure");
        }

        [Then(@"the following payments will be calculated")]
        public void ThenTheFollowingPaymentsWillBeCalculated(Table table)
        {
            var expectedPayments = table.CreateSet<Payment>().ToList();
            WaitForIt(() => PaymentEventMatcher.MatchPayments(expectedPayments, TestSession.Ukprn), "Payment event check failure");
        }

        [Then(@"the following provider payments will be generated")]
        public void ThenTheFollowingProviderPaymentsWillBeGenerated(Table table)
        {
            var expectedPayments = table.CreateSet<ProviderPayment>().ToList();
            WaitForIt(() => ProviderPaymentEventMatcher.MatchPayments(expectedPayments, TestSession.Ukprn), "ProviderPayment event check failure");
        }
    }
}
