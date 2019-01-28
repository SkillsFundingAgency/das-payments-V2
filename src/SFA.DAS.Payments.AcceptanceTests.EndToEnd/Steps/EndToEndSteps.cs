using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
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

        [Given(@"the Provider now changes the Learner details as follows")]
        public void GivenTheProviderNowChangesTheLearnerDetailsAsFollows(Table table)
        {
            AddNewIlr(table);
        }

        [Given("the Learner has now changed to \"(.*)\" as follows")]
        public void GivenTheLearnerChangesProvider(string providerId, Table table)
        {
            if (!TestSession.AtLeastOneScenarioCompleted)
            {
                TestSession.RegenerateUkprn();
                AddNewIlr(table);
            }
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
            if (TestSession.AtLeastOneScenarioCompleted)
            {
                return;
            }

            var newPriceEpisodes = table.CreateSet<Price>().ToList();
            CurrentPriceEpisodes = newPriceEpisodes;

            if (TestSession.Learners.Any(x => x.Aims.Count > 0))
            {
                foreach (var newPriceEpisode in newPriceEpisodes)
                {
                    Aim aim;
                    try
                    {
                        aim = TestSession.Learners.SelectMany(x => x.Aims)
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

            if (CurrentIlr != null)
            {
                foreach (var training in CurrentIlr)
                {
                    var aim = new Aim(training);
                    var aims = new List<Aim> { aim };
                    AddTestAims(aims);

                    if (CurrentPriceEpisodes == null)
                    {
                        aim.PriceEpisodes.Add(new Price
                        {
                            AimSequenceNumber = training.AimSequenceNumber,
                            TotalAssessmentPrice = training.TotalAssessmentPrice,
                            TotalTrainingPrice = training.TotalTrainingPrice,
                            TotalTrainingPriceEffectiveDate = training.StartDate,
                            TotalAssessmentPriceEffectiveDate = training.StartDate,
                            SfaContributionPercentage = training.SfaContributionPercentage,
                        });
                    }
                    else
                    {
                        foreach (var currentPriceEpisode in CurrentPriceEpisodes)
                        {
                            if (currentPriceEpisode.AimSequenceNumber == 0)
                            {
                                aims.Single().PriceEpisodes.Add(currentPriceEpisode);
                            }
                            else
                            {
                                var matchingAim = aims.First(x => x.AimSequenceNumber == currentPriceEpisode.AimSequenceNumber);
                                matchingAim.PriceEpisodes.Add(currentPriceEpisode);
                            }
                        }
                    }
                }
            }

            // Learner -> Aims -> Price Episodes
            foreach (var testSessionLearner in TestSession.Learners)
            {
                var learner = new FM36Learner { LearnRefNumber = testSessionLearner.LearnRefNumber };
                var learnerEarnings = earnings.Where(e => e.LearnerId == testSessionLearner.LearnerIdentifier).ToList();
                PopulateLearner(learner, testSessionLearner, learnerEarnings);
                learners.Add(learner);
            }

            var dcHelper = Scope.Resolve<DcHelper>();
            await dcHelper.SendLearnerCommands(learners, TestSession.Ukprn, AcademicYear, CollectionPeriod, TestSession.JobId, TestSession.IlrSubmissionTime);
            var matcher = new EarningEventMatcher(earnings, TestSession, CurrentCollectionPeriod, learners);
            await WaitForIt(() => matcher.MatchPayments(), "Earning event check failure");
        }

        [Then(@"only the following payments will be calculated")]
        public async Task ThenTheFollowingPaymentsWillBeCalculated(Table table)
        {
            var expectedPayments = CreatePayments(table);
            var matcher = new RequiredPaymentEventMatcher(TestSession, CurrentCollectionPeriod, expectedPayments, CurrentIlr, CurrentPriceEpisodes);
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
            var monthEndJobId = TestSession.GenerateId();
            Console.WriteLine($"Month end job id: {monthEndJobId}");
            TestSession.SetJobId(monthEndJobId);
            var monthEndCommand = new ProcessProviderMonthEndCommand
            {
                CollectionPeriod = CurrentCollectionPeriod,
                Ukprn = TestSession.Ukprn,
                JobId = monthEndJobId
            };
            //TODO: remove when DC have implemented the Month End Task
            var startedMonthEndJob = new RecordStartedProcessingMonthEndJob
            {
                JobId = monthEndJobId,
                CollectionPeriod = CollectionPeriod,
                CollectionYear = AcademicYear,
                GeneratedMessages = new List<GeneratedMessage> {new GeneratedMessage
                {
                    StartTime = DateTimeOffset.UtcNow,
                    MessageName = monthEndCommand.GetType().FullName,
                    MessageId = monthEndCommand.CommandId
                }}
            };
            await MessageSession.Send(startedMonthEndJob).ConfigureAwait(false);

            await MessageSession.Send(monthEndCommand).ConfigureAwait(false);
            var expectedPayments = table.CreateSet<ProviderPayment>().ToList();
            var matcher = new ProviderPaymentEventMatcher(CurrentCollectionPeriod, TestSession, expectedPayments);
            await WaitForIt(() => matcher.MatchPayments(), "Provider Payment event check failure");
        }

        [Then(@"no provider payments will be recorded")]
        public async Task ThenNoProviderPaymentsWillBeRecorded()
        {
            var dataContext = Container.Resolve<IPaymentsDataContext>();
            var matcher = new ProviderPaymentModelMatcher(dataContext, TestSession, CurrentCollectionPeriod);
            await WaitForUnexpected(() => matcher.MatchNoPayments(), "Payment history check failure");
        }

        [Then(@"no learner earnings should be generated")]
        public async Task ThenNoLearnerEarningsWillBeRecorded()
        {
            var matcher = new EarningEventMatcher(null, TestSession, CurrentCollectionPeriod, null);
            await WaitForUnexpected(() => matcher.MatchNoPayments(), "Earning Event check failure");
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