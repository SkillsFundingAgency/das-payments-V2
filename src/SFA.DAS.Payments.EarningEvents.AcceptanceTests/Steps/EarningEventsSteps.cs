using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Steps
{
    [Binding]
    public class EarningEventsSteps : EarningEventsStepsBase
    {
        private readonly IKeyValuePersistenceService redisService;
        private readonly IJsonSerializationService serializationService;
        private readonly ITopicPublishService<JobContextDto> topicPublishingService;

        protected ApprenticeshipContractType2EarningEvent Act2EarningEvent
        {
            get => Get<ApprenticeshipContractType2EarningEvent>();
            set => Set(value);
        }
        public EarningEventsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
            redisService = Container.Resolve<IKeyValuePersistenceService>();
            serializationService = Container.Resolve<IJsonSerializationService>();
            topicPublishingService = Container.Resolve<ITopicPublishService<JobContextDto>>();
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
            var learners = CreateLearners();
            await SendIlrSubmission(learners).ConfigureAwait(true);
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
                        .Any(period =>
                            // period.Period.Period == expectedEarning.Period &&
                            period.Amount == expectedEarning.Amount &&
                            period.PriceEpisodeIdentifier == expectedEarning.PriceEpisodeIdentifier)
                    , $"Failed to find expected earning. Price Episode: {expectedEarning.PriceEpisodeIdentifier}, Period: {expectedEarning.Period}, Type: {expectedEarning.OnProgrammeEarningType:G}, Amount: {expectedEarning.Amount}.");
            }
        }

        [Given(@"the earnings calculator generates the following FM36 price episodes:")]
        public void GivenTheEarningsCalculatorGeneratesTheFollowingFMPriceEpisodes(Table table)
        {
        }

        private List<FM36Learner> CreateLearners()
        {
            var learners = TestSession.Learners.Select(testLearner => new FM36Learner
            {
                LearnRefNumber = TestSession.Learner.LearnRefNumber,
                PriceEpisodes = new List<PriceEpisode>(),
                LearningDeliveries = new List<LearningDelivery>()
            })
                .ToList();

            learners.ForEach(learner =>
            {
                IlrLearnerEarnings.ForEach(earnings =>
                {
                    AddLearnerEarnings(learner, earnings);
                });
                Console.WriteLine($"Created learner: {learner.ToJson()}");

            });

            return learners;
        }

        private async Task SendIlrSubmission(List<FM36Learner> learners)
        {
            try
            {
                var messagePointer = Guid.NewGuid().ToString();
                var ilrSubmission = new FM36Global
                {
                    UKPRN = (int)TestSession.Ukprn,
                    Year = CollectionYear,
                    Learners = learners
                };
                var json = serializationService.Serialize(ilrSubmission);
                Console.WriteLine($"ILR Submission: {json}");
                await redisService
                    .SaveAsync(messagePointer, json)
                    .ConfigureAwait(true);

                var dto = new JobContextDto
                {
                    JobId = 1,
                    KeyValuePairs = new Dictionary<string, object>
                    {
                        {"FundingFm36Output", messagePointer},
                        {"Filename", "blah blah"},
                        {"UkPrn", TestSession.Ukprn},
                        {"Username", "Bob"}
                    },
                    SubmissionDateTimeUtc = DateTime.UtcNow,
                    TopicPointer = 0,
                    Topics = new List<TopicItemDto>
                    {
                        new TopicItemDto
                        {
                            SubscriptionName = "Validation",
                            Tasks = new List<TaskItemDto>
                                {new TaskItemDto {SupportsParallelExecution = false, Tasks = new List<string>()}}
                        }
                    }
                };

                await topicPublishingService.PublishAsync(dto, new Dictionary<string, object>(), "Validation");
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