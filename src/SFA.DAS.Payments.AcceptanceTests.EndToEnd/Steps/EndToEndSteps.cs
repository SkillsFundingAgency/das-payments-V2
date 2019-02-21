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
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core;
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
            TestSession.Providers.ForEach(p =>
            {
                var newJobId = TestSession.GenerateId();
                Console.WriteLine($"Using new job. Previous job id: { p.JobId }, new job id: {newJobId}");
                p.JobId = newJobId;
            });
        }

        [AfterScenario()]
        public void CleanUpScenario()
        {
            NewFeature = false;
        }

        [Given(@"the ""(.*)"" levy account balance in collection period (.*) is (.*)")]
        public async Task GivenTheSpecificEmployerLevyAccountBalanceInCollectionPeriodIs(
            string employerIdentifier,
            string collectionPeriod, 
            decimal levyAmount)
        {
            var employer = TestSession.GetEmployer(employerIdentifier);
            employer.Balance = levyAmount;
            await SaveLevyAccount(employer);
        }

        [Given(@"the employer levy account balance in collection period (.*) is (.*)")]
        public Task GivenTheEmployerLevyAccountBalanceInCollectionPeriodRCurrentAcademicYearIs(string collectionPeriod, decimal levyAmount)
        {
            return GivenTheSpecificEmployerLevyAccountBalanceInCollectionPeriodIs(
                TestSession.Employer.Identifier,
                collectionPeriod, 
                levyAmount);
        }

        [Given(@"the provider is providing training for the following learners")]
        public void GivenTheProviderIsProvidingTrainingForTheFollowingLearners(Table table)
        {
            CurrentIlr = table.CreateSet<Training>().ToList();
            AddTestLearners(CurrentIlr,TestSession.Ukprn);
        }

        [Given(@"the provider previously submitted the following learner details in collection period ""(.*)""")]
        public void GivenTheProviderPreviouslySubmittedTheFollowingLearnerDetailsInCollectionPeriod(string previousCollectionPeriod, Table table)
        {
            SetCollectionPeriod(previousCollectionPeriod);
            var ilr = table.CreateSet<Training>().ToList();
            PreviousIlr = ilr;
            AddTestLearners(PreviousIlr,TestSession.Ukprn);
        }

        [Given(@"the Provider now changes the Learner details as follows")]
        public void GivenTheProviderNowChangesTheLearnerDetailsAsFollows(Table table)
        {
            AddNewIlr(table,TestSession.Ukprn);
        }

        [Given("the Learner has now changed to \"(.*)\" as follows")]
        public void GivenTheLearnerChangesProvider(string providerId, Table table)
        {
            if (!TestSession.AtLeastOneScenarioCompleted)
            {
                TestSession.RegenerateUkprn();
                AddNewIlr(table, TestSession.Ukprn);
            }
        }

        [Given(@"the following learners")]
        public void GivenTheFollowingLearners(Table table)
        {
            var learners = table.CreateSet<Learner>();
            AddTestLearners(learners, TestSession.Ukprn);
        }

        [Given(@"aims details are changed as follows")]
        public void GivenAimsDetailsAreChangedAsFollows(Table table)
        {
            AddTestAims(table.CreateSet<Aim>().ToList(), TestSession.Provider.Ukprn);
        }

        [Given(@"the following aims")]
        public void GivenTheFollowingAims(Table table)
        {
            var aims = table.CreateSet<Aim>().ToList();
            AddTestAims(aims, TestSession.Provider.Ukprn);
        }

        private static readonly HashSet<long> PriceEpisodesProcessedForJob = new HashSet<long>();

        [Given(@"price details are changed as follows")]
        public void GivenPriceDetailsAreChangedAsFollows(Table table)
        {
            PriceEpisodesProcessedForJob.Remove(TestSession.JobId);
            GivenPriceDetailsAsFollows(table);
        }

        [Given(@"the following commitments exist")]
        [Given(@"the Commitment details are changed as follows")]
        public async Task GivenTheFollowingCommitmentsExist(Table table)
        {
            if (!TestSession.AtLeastOneScenarioCompleted)
            {
                var commitments = table.CreateSet<Commitment>().ToList();
                await AddTestCommitments(commitments);
            }
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
           await GeneratedAndValidateEarnings( table,  TestSession.Provider);
        }

        [Then(@"only the following payments will be calculated")]
        public async Task ThenTheFollowingPaymentsWillBeCalculated(Table table)
        {
            await MatchCalculatedPayments(table,TestSession.Provider);
        }

        [Then(@"at month end only the following payments will be calculated")]
        public async Task ThenAtMonthEndOnlyTheFollowingPaymentsWillBeCalculated(Table table)
        {
            await MatchCalculatedPayments(table, TestSession.Provider);

            var monthEndJobId = TestSession.GenerateId();
            Console.WriteLine($"Month end job id: {monthEndJobId}");
            TestSession.Provider.JobId = monthEndJobId;
            TestSession.MonthEndJobIdGenerated = true;

            foreach (var employer in TestSession.Employers)
            {
                var processLevyFundsAtMonthEndCommand = new ProcessLevyPaymentsOnMonthEndCommand
                {
                    JobId = TestSession.JobId,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = AcademicYear, Period = CollectionPeriod },
                    RequestTime = DateTime.Now,
                    SubmissionDate = TestSession.IlrSubmissionTime,
                    EmployerAccountId = employer.AccountId,
                };

                await MessageSession.Send(processLevyFundsAtMonthEndCommand).ConfigureAwait(false);
            }
        }

        [Then(@"no payments will be calculated")]
        public async Task ThenNoPaymentsWillBeCalculated()
        {
            var matcher = new RequiredPaymentEventMatcher(TestSession.Provider, CurrentCollectionPeriod);
            await WaitForUnexpected(() => matcher.MatchNoPayments(), "Required Payment event check failure");
        }

        [Then(@"only the following provider payments will be generated")]
        public async Task ThenOnlyTheFollowingProviderPaymentsWillBeGenerated(Table table)
        {
            await StartMonthEnd(TestSession.Provider);
            await MatchOnlyProviderPayments(table, TestSession.Provider);
        }

        [Then(@"at month end only the following provider payments will be generated")]
        public async Task ThenTheFollowingProviderPaymentsWillBeGenerated(Table table)
        {
            await StartMonthEnd(TestSession.Provider);
            await MatchOnlyProviderPayments(table, TestSession.Provider);
        }

        [Then(@"no provider payments will be recorded")]
        public async Task ThenNoProviderPaymentsWillBeRecorded()
        {
            var matcher = new ProviderPaymentModelMatcher(TestSession.Provider,DataContext, TestSession, CurrentCollectionPeriod);
            await WaitForUnexpected(() => matcher.MatchNoPayments(), "Payment history check failure");
        }

        [Then(@"no learner earnings should be generated")]
        public async Task ThenNoLearnerEarningsWillBeRecorded()
        {
            var matcher =  new EarningEventMatcher(TestSession.Provider,CurrentPriceEpisodes, CurrentIlr, null, TestSession, CurrentCollectionPeriod, null);
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
            var matcher = new ProviderPaymentEventMatcher(TestSession.Provider,CurrentCollectionPeriod, TestSession);
            await WaitForUnexpected(() => matcher.MatchNoPayments(), "Provider Payment event check failure");
        }
    }
}
