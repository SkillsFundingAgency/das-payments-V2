using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Steps
{
    [Binding]
    public class ProviderPaymentsSteps: ProviderPaymentsStepsBase
    {
        public ProviderPaymentsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"a learner is undertaking a training with a training provider")]
        public void GivenALearnerIsUndertakingATrainingWithATrainingProvider()
        {
            // Use Auto Generated Learning Ref
        }

        [Given(@"the SFA contribution percentage is (.*)%")]
        public void GivenTheSFAContributionPercentageIs(decimal sfaContribution)
        {
            SfaContributionPercentage = sfaContribution / 100;
        }

        [Given(@"the current collection period is R(.*)")]
        public void GivenTheCurrentCollectionPeriodIsR(byte period)
        {
            CollectionPeriod = period;
        }

        [Given(@"the funding source service generates the following contract type (.*) payments:")]
        public void GivenTheFundingSourceServiceGeneratesTheFollowingContractTypePayments(byte contractType, Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"the funding source payments event are received")]
        public void WhenTheFundingSourcePaymentsEventAreReceived()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the provider payments service will store the following payments:")]
        public void ThenTheProviderPaymentsServiceWillStoreTheFollowingPayments(Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"at month end the provider payments service will publish the following payments")]
        public void ThenAtMonthEndTheProviderPaymentsServiceWillPublishTheFollowingPayments(Table table)
        {
            ScenarioContext.Current.Pending();
        }



    }
}