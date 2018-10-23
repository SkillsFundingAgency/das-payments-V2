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
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using EarningEvent = SFA.DAS.Payments.EarningEvents.AcceptanceTests.Data.EarningEvent;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Steps
{
    [Binding]
    public class EarningEventsSteps : EarningEventsStepsBase
    {
        private readonly IKeyValuePersistenceService redisService;
        private readonly IJsonSerializationService serializationService;
        protected ApprenticeshipContractType2EarningEvent Act2EarningEvent
        {
            get => Get<ApprenticeshipContractType2EarningEvent>();
            set => Set(value);
        }
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
            //var command = new ProcessLearnerCommand
            //{
            //    Ukprn = TestSession.Ukprn,
            //    CollectionYear = CollectionYear,
            //    CollectionPeriod = CollectionPeriod,
            //    JobId = TestSession.JobId,
            //    RequestTime = DateTime.UtcNow,
            //    Learner = new FM36Learner
            //    {
            //        LearnRefNumber = TestSession.Learner.LearnRefNumber,
            //        PriceEpisodes = new List<PriceEpisode>(),
            //        LearningDeliveries = new List<LearningDelivery>()
            //    }
            //};
            //IlrLearnerEarnings.ForEach(earnings =>
            //{
            //    AddLearnerEarnings(command.Learner, earnings);
            //});
            //await MessageSession.Send(command, new SendOptions());
            var learner = CreateLearner();
            await SendIlrSubmission(learner).ConfigureAwait(true);
        }

        [Then(@"the earning events service will generate a contract type 2 earnings event for the learner")]
        public void ThenTheEarningEventsServiceWillGenerateAContractTypeEarningsEventForTheLearner()
        {
            WaitForIt(() =>
                {
                    var act2EarningEvent = Handlers.ApprenticeshipContractType2EarningEventHandler.ReceivedEvents.FirstOrDefault(ev =>
                        ev.Learner.ReferenceNumber == TestSession.Learner.LearnRefNumber);
                    if (act2EarningEvent == null) return false;
                    Act2EarningEvent = act2EarningEvent;
                    Console.WriteLine($"Found learner earning event: {Act2EarningEvent.ToJson()}");
                    return true;

                }, $"Failed to find the ACT2 earning event for learner {TestSession.Learner.LearnRefNumber}");
        }

        [Then(@"the earnings event will contain the following earnings")]
        public void ThenTheEarningsEventWillContainTheFollowingEarnings(Table table)
        {
            var expectedEarnings = table.CreateSet<ExpectedEarning>();
            foreach (var expectedEarning in expectedEarnings)
            {
                Assert.IsTrue(
                    Act2EarningEvent.OnProgrammeEarnings
                        .Where(onProgEarning => onProgEarning.Type == expectedEarning.OnProgrammeEarningType)
                        .SelectMany(onProgEarning => onProgEarning.Periods)
                        .Any(period => period.Period == expectedEarning.Period &&
                                       period.Amount == expectedEarning.Amount &&
                                       period.PriceEpisodeIdentifier == expectedEarning.PriceEpisodeIdentifier)
                    , $"Failed to find expected earning. Price Episode: {expectedEarning.PriceEpisodeIdentifier}, Period: {expectedEarning.Period}, Type: {expectedEarning.OnProgrammeEarningType:G}, Amount: {expectedEarning.Amount}.");
            }
        }
        
        [Given(@"the earnings calculator generates the following FM36 price episodes:")]
        public void GivenTheEarningsCalculatorGeneratesTheFollowingFMPriceEpisodes(Table table)
        {
        }

        private FM36Learner CreateLearner()
        {
            var learner = new FM36Learner
            {
                LearnRefNumber = TestSession.Learner.LearnRefNumber,
                PriceEpisodes = new List<PriceEpisode>(),
                LearningDeliveries = new List<LearningDelivery>()
            };
            IlrLearnerEarnings.ForEach(earnings =>
            {
                AddLearnerEarnings(learner, earnings);
            });
            Console.WriteLine($"Created learner: {learner.ToJson()}");
            return learner;
        }

        private async Task SendIlrSubmission(FM36Learner learner)
        {
            try
            {
                var messagePointer = Guid.NewGuid().ToString();
                var ilrSubmission = new FM36Global
                {
                    UKPRN = (int)TestSession.Ukprn,
                    Year = CollectionYear,
                    Learners = new List<FM36Learner> { learner }
                };
                var json = serializationService.Serialize(ilrSubmission);
                Console.WriteLine($"ILR Submission: {json}");
                await redisService
                    .SaveAsync(messagePointer, json)
                    .ConfigureAwait(true);

                var jobContextMessage = new JobContextMessage
                {
                    JobId = 1,
                    KeyValuePairs = new Dictionary<string, object> { { "FundingFm36Output", messagePointer } },
                    SubmissionDateTimeUtc = DateTime.UtcNow,
                    TopicPointer = 1,
                };

                var serialisedMessage = serializationService.Serialize(jobContextMessage);
                Console.WriteLine($"Job context message: {serialisedMessage}");
                var topicClient = TopicClient.CreateFromConnectionString(TestConfiguration.DcServiceBusConnectionString,
                    TestConfiguration.TopicName);
                var brokeredMessage = new BrokeredMessage(serialisedMessage);
                await topicClient.SendAsync(brokeredMessage).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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