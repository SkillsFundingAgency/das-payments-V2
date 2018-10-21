using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Serialization.Interfaces;
using FluentAssertions;
using Microsoft.ServiceBus.Messaging;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.AcceptanceTests.Data;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Steps
{
    [Binding]
    public class EarningEventsSteps : EarningEventsStepsBase
    {
        private readonly IKeyValuePersistenceService redisService;
        private readonly IJsonSerializationService serializationService;

        public EarningEventsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
            redisService = Container.Resolve<IKeyValuePersistenceService>();
            serializationService = Container.Resolve<IJsonSerializationService>();
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
            await MessageSession.Send(command, new SendOptions());
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





        [Given(@"the earnings calculator generates the following FM36 price episodes:")]
        public void GivenTheEarningsCalculatorGeneratesTheFollowingFMPriceEpisodes(Table table)
        {
        }


        [When(@"earning calculator event is received")]
        public async void WhenEarningCalculatorEventIsReceived()
        {
            var input = new FM36Global();
            input.Learners = new List<FM36Learner>();

            var learner = new FM36Learner
            {
                LearnRefNumber = TestSession.Learner.LearnRefNumber,
                PriceEpisodes = new List<PriceEpisode>()
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
            try
            {
                var messagePointer = Guid.NewGuid().ToString();

                await redisService.SaveAsync(messagePointer, serializationService.Serialize(input)).ConfigureAwait(true);

                var inputMessage = new JobContextMessage
                {
                    JobId = 1,
                    KeyValuePairs = new Dictionary<string, object> { { "FundingFm36Output", messagePointer } },
                    SubmissionDateTimeUtc = DateTime.Now,
                    TopicPointer = 1,
                };

                var serialisedMessage = serializationService.Serialize(inputMessage);

                var topicClient = TopicClient.CreateFromConnectionString(TestConfiguration.DcServiceBusConnectionString,
                    TestConfiguration.TopicName);
                var brokeredMessage = new BrokeredMessage(serialisedMessage);

                await topicClient.SendAsync(brokeredMessage).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
        }

        [Then(@"the earning events component will generate the following earning events:")]
        public void ThenTheEarningEventsComponentWillGenerateTheFollowingEarningEvents(Table table)
        {
            var expectedFundingSourcePaymentEvents = table.CreateSet<EarningEvent>();
            WaitForIt(() =>
                {
                    return Handlers.ApprenticeshipContractType2EarningEventHandler.ReceivedEvents.Any(ev =>
                        ev.Learner.ReferenceNumber == TestSession.Learner.LearnRefNumber);
                }, "Failed to find all the earning events");
        }
    }
}