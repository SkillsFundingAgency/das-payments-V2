using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Steps
{
    [Binding]
    public class EarningEventsSteps : EarningEventsStepsBase
    {
        public EarningEventsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"the earnings calculator generates the following FM36 price episodes:")]
        public void GivenTheEarningsCalculatorGeneratesTheFollowingFMPriceEpisodes(Table table)
        {
            ScenarioContext.Current.Pending();
        }


        [When(@"earning calculator event is received")]
        public async void WhenEarningCalculatorEventIsReceived()
        {
          
        }

        [Then(@"the earning events component will generate the following earning events:")]
        public void ThenTheEarningEventsComponentWillGenerateTheFollowingEarningEvents(Table table)
        {
            ScenarioContext.Current.Pending();
        }

    }
}