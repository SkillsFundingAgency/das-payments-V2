using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FluentAssertions;
using NServiceBus;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Steps
{
    [Binding]
    public class EarningsEventsSteps: EarningsEventsStepsBase
    {
        public EarningsEventsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }
        [Given(@"the earnings are for the current collection year")]
        public void GivenThePaymentsAreForTheCurrentCollectionYear()
        {
            var year = DateTime.Today.Year - 2000;
            CollectionYear = DateTime.Today.Month < 9 ? $"{year - 1}{year}" : $"{year}{year + 1}";
        }

        [Given(@"the current collection period is (.*)")]
        public void GivenTheCurrentProcessingPeriodIs(byte period)
        {
            CollectionPeriod = period;
        }

        [Given(@"the earnings are for a test learner and a test provider")]
        public void GivenTheEarningsAreForATestLearnerAndATestProvider()
        {
            //TODO map to TestContextProvider
        }

        [Given(@"the SFA contribution percentage is (.*)%")]
        public void GivenTheSFAContributionPercentageIs(decimal sfaContribution)
        {
            SfaContributionPercentage = sfaContribution / 100;
        }

        [Given(@"the Earnings Calc has generated the following learner earnings")]
        public void GivenTheEarningsCalcHasGeneratedTheFollowingLearnerEarnings(Table table)
        {
            var ilrLearnerEarnings = table.CreateSet<LearnerEarnings>().ToList();
            ilrLearnerEarnings.Should().NotBeEmpty();
            IlrLearnerEarnings = ilrLearnerEarnings;
        }

        [When(@"the ILR is submitted and the learner earnings are sent to the earning events service")]
        public async Task WhenTheILRIsSubmittedAndTheLearnerEarningsAreSentToTheEarningEventsService()
        {
            var command = new ProcessLearnerCommand
            {
                Ukprn = TestSession.Ukprn,
                CollectionYear = CollectionYear,
                CollectionPeriod = CollectionPeriod,
                JobId = TestSession.JobId,
                RequestTime = DateTime.UtcNow,
                Learner = new FM36Learner
                {
                    LearnRefNumber = TestSession.Learner.LearnRefNumber,
                    PriceEpisodes = new List<PriceEpisode>(),
                    LearningDeliveries = new List<LearningDelivery>()
                }
            };
            IlrLearnerEarnings.ForEach(earnings =>
            {
                AddLearnerEarnings(command.Learner, earnings);
            });
            await MessageSession.Send(command, new SendOptions ());
        }

        [Then(@"the earning events service will generate a contract type (.*) earnings event for the learner")]
        public void ThenTheEarningEventsServiceWillGenerateAContractTypeEarningsEventForTheLearner(int p0)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the earnings event will contain the following earnings")]
        public void ThenTheEarningsEventWillContainTheFollowingEarnings(Table table)
        {
            ScenarioContext.Current.Pending();
        }

    }
}