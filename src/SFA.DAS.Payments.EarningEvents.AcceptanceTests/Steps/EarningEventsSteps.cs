using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NServiceBus;
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
            var input = new FM36Global();

            var learner = new FM36Learner
            {
                LearnRefNumber = TestSession.Learner.LearnRefNumber, PriceEpisodes = new List<PriceEpisode>()
            };

            var priceEpisode = new PriceEpisode
            {
                PriceEpisodeIdentifier = "p1",
                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                {
                    new PriceEpisodePeriodisedValues
                    {
                        Period1 = 100M,
                        Period2 = 100M,
                        Period3 = 100M,
                        Period4 = 100M,
                        Period5 = 100M,
                        Period6 = 100M,
                        Period7 = 100M,
                        Period8 = 100M,
                        Period9 = 100M,
                        Period10 = 100M,
                        Period11 = 100M,
                        Period12 = 100M
                    }
                }
            };

            learner.PriceEpisodes.Add(priceEpisode);
            input.Learners.Add(learner);

            await MessageSession.Send(input).ConfigureAwait(false);
        }

        [Then(@"the earning events component will generate the following earning events:")]
        public void ThenTheEarningEventsComponentWillGenerateTheFollowingEarningEvents(Table table)
        {
            ScenarioContext.Current.Pending();
        }

    }
}