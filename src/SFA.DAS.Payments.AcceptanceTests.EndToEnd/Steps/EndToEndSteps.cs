using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Application.Repositories;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Learner = SFA.DAS.Payments.AcceptanceTests.Core.Data.Learner;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class EndToEndSteps : EndToEndStepsBase
    {
        public EndToEndSteps(FeatureContext context) : base(context)
        {
        }

        [BeforeScenario()]
        public void ResetJob()
        {
            if (!Context.ContainsKey("new_feature"))
                NewFeature = true;
            var newJobId = TestSession.GenerateId();
            Console.WriteLine($"Using new job. Previous job id: {TestSession.JobId}, new job id: {newJobId}");
            TestSession.SetJobId(newJobId);
        }

        [AfterScenario()]
        public void CleanUpScenario()
        {
            NewFeature = false;
        }

        [Given(@"the provider is providing training for the following learners")]
        public void GivenTheProviderIsProvidingTrainingForTheFollowingLearners(Table table)
        {
            CurrentIlr = table.CreateSet<Training>().ToList();
            AddTestLearners(CurrentIlr);
        }

        [Given(@"the provider previously submitted the following learner details in collection period ""(.*)""")]
        public void GivenTheProviderPreviouslySubmittedTheFollowingLearnerDetailsInCollectionPeriod(string previousCollectionPeriod, Table table)
        {
            SetCollectionPeriod(previousCollectionPeriod);
            var ilr = table.CreateSet<Training>().ToList();
            PreviousIlr = ilr;
            AddTestLearners(PreviousIlr);
        }

        [Given(@"the following learners")]
        public void GivenTheFollowingLearners(Table table)
        {
            var learners = table.CreateSet<Learner>();
            AddTestLearners(learners);
        }

        [Given(@"aims details are changed as follows")]
        public void GivenAimsDetailsAreChangedAsFollows(Table table)
        {
            AimsProcessedForJob.Remove(TestSession.JobId);
            AddTestAims(table.CreateSet<Aim>().ToList());
        }

        [Given(@"the following aims")]
        public void GivenTheFollowingAims(Table table)
        {
            var aims = table.CreateSet<Aim>().ToList();
            AddTestAims(aims);
        }

        private static readonly HashSet<long> PriceEpisodesProcessedForJob = new HashSet<long>();

        [Given(@"price details are changed as follows")]
        public void GivenPriceDetailsAreChangedAsFollows(Table table)
        {
            PriceEpisodesProcessedForJob.Remove(TestSession.JobId);
            GivenPriceDetailsAsFollows(table);
        }


        [Given(@"price details as follows")]
        public void GivenPriceDetailsAsFollows(Table table)
        {
            if (PriceEpisodesProcessedForJob.Contains(TestSession.JobId) || !NewFeature)
            {
                return;
            }

            PriceEpisodesProcessedForJob.Add(TestSession.JobId);

            var newPriceEpisodes = table.CreateSet<Price>().ToList();
            CurrentPriceEpisodes = newPriceEpisodes;

            if (TestSession.Learners.Any(x => x.Aims.Count > 0))
            {
                foreach (var newPriceEpisode in newPriceEpisodes)
                {
                    Aim aim;
                    try
                    {
                        aim = TestSession.Learners
                            .SelectMany(x => x.Aims)
                            .SingleOrDefault(x => x.AimSequenceNumber == newPriceEpisode.AimSequenceNumber);
                    }
                    catch (Exception)
                    {
                        throw new Exception("There are too many aims with the same sequence number");
                    }

                    if (aim == null)
                    {
                        throw new Exception("There is a price episode without a matching aim");
                    }

                    aim.PriceEpisodes.Add(newPriceEpisode);
                }
            }
        }

        [Then(@"the following learner earnings should be generated")]
        public async Task ThenTheFollowingLearnerEarningsShouldBeGenerated(Table table)
        {
            var earnings = CreateEarnings(table);
            var learners = new List<FM36Learner>();

            if (CurrentIlr == null)
            {
                // Learner -> Aims -> Price Episodes
                foreach (var testSessionLearner in TestSession.Learners)
                {
                    var learner = new FM36Learner { LearnRefNumber = testSessionLearner.LearnRefNumber };
                    var learnerEarnings = earnings.Where(e => e.LearnerId == testSessionLearner.LearnerIdentifier).ToList();
                    PopulateLearner(learner, testSessionLearner, learnerEarnings);

                    var command = new ProcessLearnerCommand
                    {
                        Learner = learner,
                        CollectionPeriod = CurrentCollectionPeriod.Period,
                        CollectionYear = CollectionYear,
                        Ukprn = TestSession.Ukprn,
                        JobId = TestSession.JobId,
                        IlrSubmissionDateTime = TestSession.IlrSubmissionTime,
                        RequestTime = DateTimeOffset.UtcNow,
                        SubmissionDate = TestSession.IlrSubmissionTime, //TODO: ????          
                    };

                    //                    Console.WriteLine($"Sending process learner command to the earning events service. Command: {command.ToJson()}");
                    //                    await MessageSession.Send(command);

                    learners.Add(learner);
                }
            }
            else
            {
                foreach (var training in CurrentIlr)
                {
                    var learnerId = training.LearnerId;
                    var learner = new FM36Learner { LearnRefNumber = TestSession.GetLearner(learnerId).LearnRefNumber };
                    var learnerEarnings = earnings.Where(e => e.LearnerId == learnerId).ToList();

                    PopulateLearner(learner, training, learnerEarnings);

                    var command = new ProcessLearnerCommand
                    {
                        Learner = learner,
                        CollectionPeriod = CurrentCollectionPeriod.Period,
                        CollectionYear = CollectionYear,
                        Ukprn = TestSession.Ukprn,
                        JobId = TestSession.JobId,
                        IlrSubmissionDateTime = TestSession.IlrSubmissionTime,
                        RequestTime = DateTimeOffset.UtcNow,
                        SubmissionDate = TestSession.IlrSubmissionTime, //TODO: ????                    
                    };

                    //                    Console.WriteLine($"Sending process learner command to the earning events service. Command: {command.ToJson()}");
                    //                    await MessageSession.Send(command);

                    learners.Add(learner);
                }
            }
            var dcHelper = Container.Resolve<DcHelper>();
            await dcHelper.SendIlrSubmission(learners, TestSession.Ukprn, CollectionYear, CollectionPeriod, TestSession.JobId);
            var matcher = new EarningEventMatcher(earnings, TestSession, CurrentCollectionPeriod, learners);
            await WaitForIt(() => matcher.MatchPayments(), "Earning event check failure");
        }


        [Then(@"only the following payments will be calculated")]
        public async Task ThenTheFollowingPaymentsWillBeCalculated(Table table)
        {
            var expectedPayments = CreatePayments(table);
            var matcher = new RequiredPaymentEventMatcher(TestSession, CurrentCollectionPeriod, expectedPayments);
            await WaitForIt(() => matcher.MatchPayments(), "Required Payment event check failure");
        }

        [Then(@"no payments will be calculated")]
        public async Task ThenNoPaymentsWillBeCalculated()
        {
            var matcher = new RequiredPaymentEventMatcher(TestSession, CurrentCollectionPeriod);
            await WaitForUnexpected(() => matcher.MatchNoPayments(), "Required Payment event check failure");
        }

        [Then(@"at month end only the following provider payments will be generated")]
        public async Task ThenTheFollowingProviderPaymentsWillBeGenerated(Table table)
        {
            var monthEndCommand = new ProcessProviderMonthEndCommand
            {
                CollectionPeriod = CurrentCollectionPeriod,
                Ukprn = TestSession.Ukprn,
                JobId = TestSession.JobId
            };
            await MessageSession.Send(monthEndCommand);
            var expectedPayments = table.CreateSet<ProviderPayment>().ToList();
            var matcher = new ProviderPaymentEventMatcher(CurrentCollectionPeriod, TestSession, expectedPayments);
            await WaitForIt(() => matcher.MatchPayments(), "Provider Payment event check failure");
        }

        [Then(@"no provider payments will be recorded")]
        public async Task ThenNoProviderPaymentsWillBeRecorded()
        {
            var dataContext = Container.Resolve<IPaymentsDataContext>();
            var matcher = new ProviderPaymentModelMatcher(dataContext, TestSession, CurrentCollectionPeriod.Name);
            await WaitForUnexpected(() => matcher.MatchNoPayments(), "Payment history check failure");
        }

        [Then(@"at month end no provider payments will be generated")]
        public async Task ThenAtMonthEndNoProviderPaymentsWillBeGenerated()
        {
            var monthEndCommand = new ProcessProviderMonthEndCommand
            {
                CollectionPeriod = CurrentCollectionPeriod,
                Ukprn = TestSession.Ukprn,
                JobId = TestSession.JobId
            };
            await MessageSession.Send(monthEndCommand);
            var matcher = new ProviderPaymentEventMatcher(CurrentCollectionPeriod, TestSession);
            await WaitForUnexpected(() => matcher.MatchNoPayments(), "Provider Payment event check failure");
        }
    }
}