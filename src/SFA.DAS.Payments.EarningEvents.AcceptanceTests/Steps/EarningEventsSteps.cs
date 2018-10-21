using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.ServiceBus.Messaging;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.AcceptanceTests.Data;
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