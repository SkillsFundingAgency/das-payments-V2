using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core.Builders;
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
                Console.WriteLine($"Using new job. Previous job id: { p.JobId }, new job id: {newJobId} for ukprn: {p.Ukprn}");
                p.JobId = newJobId;
            });
        }

        [AfterScenario()]
        public void CleanUpScenario()
        {
            NewFeature = false;
        }

        [Given(@"the ""(.*)"" levy account balance in collection period (.*) is (.*)")]
        public async Task GivenTheSpecificEmployerLevyAccountBalanceInCollectionPeriodIs(string employerIdentifier, string collectionPeriodText, decimal levyAmount)
        {
            var employer = TestSession.GetEmployer(employerIdentifier);
            employer.Balance = levyAmount;
            employer.IsLevyPayer = true;
            await SaveLevyAccount(employer).ConfigureAwait(false);
            SetCollectionPeriod(collectionPeriodText);
        }

        [Given(@"the remaining transfer allowance for ""(.*)"" is (.*)")]
        public async Task GivenTheRemainingTransferAllowanceForIs(string employerIdentifier, decimal remainingTransferAllowance)
        {
            var employer = TestSession.GetEmployer(employerIdentifier);
            employer.TransferAllowance = remainingTransferAllowance;
            await SaveLevyAccount(employer).ConfigureAwait(false);
        }


        [Given(@"the employer levy account balance in collection period (.*) is (.*)")]
        public Task GivenTheEmployerLevyAccountBalanceInCollectionPeriodRCurrentAcademicYearIs(string collectionPeriod, decimal levyAmount)
        {
            return GivenTheSpecificEmployerLevyAccountBalanceInCollectionPeriodIs(
                TestSession.Employer.Identifier,
                collectionPeriod,
                levyAmount);
        }

        [Given(@"the provider previously submitted the following learner details in collection period ""(.*)""")]
        public void GivenTheProviderPreviouslySubmittedTheFollowingLearnerDetailsInCollectionPeriod(string previousCollectionPeriod, Table table)
        {
            SetCollectionPeriod(previousCollectionPeriod);
            var ilr = table.CreateSet<Training>().ToList();
            PreviousIlr = ilr;
            AddTestLearners(PreviousIlr, TestSession.Ukprn);
        }

        [Given(@"the provider is providing training for the following learners")]
        [Given(@"the Provider now changes the Learner details as follows")]
        public void GivenTheProviderNowChangesTheLearnerDetailsAsFollows(Table table)
        {
            AddNewIlr(table, TestSession.Ukprn);
        }

        [Given(@"the provider ""(.*)"" is providing training for the following learners")]
        [Given(@"the ""(.*)"" is providing training for the following learners")]
        [Given(@"the ""(.*)"" now changes the Learner details as follows")]
        public void GivenTheNowChangesTheLearnerDetailsAsFollows(string providerIdentifier, Table table)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            AddNewIlr(table, provider.Ukprn);
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

        [Given(@"price details are changed as follows")]
        public void GivenPriceDetailsAreChangedAsFollows(Table table)
        {
            GivenPriceDetailsAsFollows(table);
        }

        [Given(@"the provider prority order is")]
        public void GivenTheProviderPriorityOrder(Table table)
        {
            
        }

        [Given(@"the following commitments exist")]
        [Given(@"the following apprenticeships exist")]
        [Given(@"the Commitment details are changed as follows")]
        [Given(@"the Apprenticeship details are changed as follows")]
        [Given(@"the following apprenticeships exist")]
        public async Task GivenTheFollowingApprenticeshipsExist(Table table)
        {
            if (!TestSession.AtLeastOneScenarioCompleted)
            {
                var apprenticeships = table.CreateSet<Apprenticeship>().ToList();
                await AddOrUpdateTestApprenticeships(apprenticeships).ConfigureAwait(false);
            }
        }

        [Given(@"the apprenticeships status changes as follows")]
        public async Task GivenTheApprenticeshipsStatusChangesAsFollows(Table table)
        {
            var statuses = table.CreateSet<ApprenticeshipStatusPeriod>();
            var validStatuses = statuses.Select(status => new
            {
                status.Status,
                CollectionPeriod = new CollectionPeriodBuilder().WithSpecDate(status.CollectionPeriod).Build(),
                status.Identifier,
                status.StoppedDate
            })
                .Where(status => status.CollectionPeriod.AcademicYear == CurrentCollectionPeriod.AcademicYear &&
                                 status.CollectionPeriod.Period == CurrentCollectionPeriod.Period)
                .ToList();
            foreach (var validStatus in validStatuses)
            {
                var apprenticeship = Apprenticeships.FirstOrDefault(appr => appr.Identifier == validStatus.Identifier);
                if (apprenticeship == null)
                    throw new InvalidOperationException($"Apprenticeship not found. Identifier: {validStatus.Identifier}");
                Console.WriteLine($"Updating status of apprenticeship. Identifier: {validStatus.Identifier}, apprenticeship id: {apprenticeship.ApprenticeshipId}, status: {validStatus.Status}");
                await UpdateApprenticeshipStatus(apprenticeship.ApprenticeshipId, validStatus.Status, validStatus.StoppedDate);
            }
        }

        [Given(@"the employer IsLevyPayer flag is (.*)")]
        public async Task GivenTheEmployerIsLevyPayerFlagIsFalse(bool isLevyFlag)
        {
            var employer = TestSession.Employer;
            employer.IsLevyPayer = isLevyFlag;
            await SaveLevyAccount(employer).ConfigureAwait(false);
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

        [Given("the following capping will apply to the price episodes")]
        public void GivenTheFollowingCappingWillApply(Table table)
        {
            AddLevyAccountPriorities.ProcessTable(table, TestSession, CurrentCollectionPeriod, DataContext);
        }

        [Then(@"the following learner earnings should be generated")]
        public async Task ThenTheFollowingLearnerEarningsShouldBeGenerated(Table table)
        {
            await GeneratedAndValidateEarnings(table, TestSession.Provider).ConfigureAwait(false);
        }

        [Then(@"the following learner earnings should be generated for ""(.*)""")]
        public async Task ThenTheFollowingLearnerEarningsShouldBeGeneratedFor(string providerIdentifier, Table table)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            await GeneratedAndValidateEarnings(table, provider).ConfigureAwait(false);
        }

        [Then(@"only the following payments will be calculated")]
        public async Task ThenTheFollowingPaymentsWillBeCalculated(Table table)
        {
            await MatchRequiredPayments(table, TestSession.Provider).ConfigureAwait(false);
        }

        [Then(@"at month end only the following payments will be calculated")]
        public async Task ThenAtMonthEndOnlyTheFollowingPaymentsWillBeCalculated(Table table)
        {
            await ValidateRequiredPaymentsAtMonthEnd(table, TestSession.Provider).ConfigureAwait(false);
        }

        [Then(@"at month end only the following payments will be calculated for ""(.*)""")]
        public async Task ThenAtMonthEndOnlyTheFollowingPaymentsWillBeCalculatedFor(string providerIdentifier, Table table)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            await MatchRequiredPayments(table, provider).ConfigureAwait(false);
        }

        [Then(@"no payments will be calculated")]
        public async Task ThenNoPaymentsWillBeCalculated()
        {
            var matcher = new RequiredPaymentEventMatcher(TestSession.Provider, CurrentCollectionPeriod);
            await WaitForUnexpected(() => matcher.MatchNoPayments(), "Required Payment event check failure").ConfigureAwait(false);
        }

        [Then(@"at month end no payments will be calculated for ""(.*)""")]
        public async Task ThenAtMonthEndNoPaymentsWillBeCalculatedForProvider(string providerIdentifier)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            var matcher = new RequiredPaymentEventMatcher(provider, CurrentCollectionPeriod);
            await WaitForUnexpected(() => matcher.MatchNoPayments(), "Required Payment event check failure").ConfigureAwait(false);
        }

        [Then(@"only the following provider payments will be generated")]
        public async Task ThenOnlyTheFollowingProviderPaymentsWillBeGenerated(Table table)
        {
            await StartMonthEnd(TestSession.Provider).ConfigureAwait(false);
            await MatchOnlyProviderPayments(table, TestSession.Provider).ConfigureAwait(false);
        }

        [Then(@"only the following ""(.*)"" payments will be generated")]
        public async Task ThenOnlyTheFollowingPaymentsWillBeGenerated(string providerIdentifier, Table table)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            await MatchOnlyProviderPayments(table, provider).ConfigureAwait(false);
        }

        [Then(@"no provider payments will be generated")]
        public async Task ThenNoProviderPaymentsWillBeGenerated()
        {
            var provider = TestSession.Provider;
            await ThenNoProviderPaymentsWillBeGenerated(provider.Identifier);
        }

        [Then(@"no ""(.*)"" payments will be generated")]
        public async Task ThenNoProviderPaymentsWillBeGenerated(string providerIdentifier)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            var matcher = new ProviderPaymentEventMatcher(provider, CurrentCollectionPeriod, TestSession);
            await WaitForUnexpected(() => matcher.MatchNoPayments(), "Provider Payment event check failure");
        }

        [Then(@"Month end is triggered")]
        public async Task WhenMonthEndIsTriggered()
        {
            await SendLevyMonthEnd().ConfigureAwait(false);

            foreach (var provider in TestSession.Providers)
            {
                await StartMonthEnd(provider).ConfigureAwait(false);
            }
        }

        [Then(@"at month end only the following provider payments will be generated")]
        public async Task ThenTheFollowingProviderPaymentsWillBeGenerated(Table table)
        {
            await WhenMonthEndIsTriggered().ConfigureAwait(false);
            await ThenOnlyTheFollowingPaymentsWillBeGenerated(TestSession.Provider.Identifier, table).ConfigureAwait(false);
        }

        [Then(@"no learner earnings should be generated")]
        public async Task ThenNoLearnerEarningsWillBeRecorded()
        {
            var matcher = new EarningEventMatcher(TestSession.Provider, CurrentPriceEpisodes, CurrentIlr, null, TestSession, CurrentCollectionPeriod, null);
            await WaitForUnexpected(() => matcher.MatchNoPayments(), "Earning Event check failure").ConfigureAwait(false);
        }

        [Then(@"at month end no provider payments will be generated")]
        public async Task ThenAtMonthEndNoProviderPaymentsWillBeGenerated()
        {
            await WhenMonthEndIsTriggered().ConfigureAwait(false);
            await ThenNoProviderPaymentsWillBeGenerated(TestSession.Provider.Identifier).ConfigureAwait(false);
        }

        [Then(@"the following data lock failures were generated")]
        public async Task ThenOnlyTheFollowingNonPayableEarningsWillBeGenerated(Table table)
        {
            await ValidateDataLockError(table, TestSession.Provider).ConfigureAwait(false);
        }

        [Then(@"the following data lock failures were generated  for ""(.*)""")]
        public async Task ThenTheFollowingDataLockFailuresWereGeneratedForAsync(string providerIdentifier, Table table)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            await ValidateDataLockError(table, provider).ConfigureAwait(false);
        }
       
        private async Task ValidateDataLockError(Table table, Provider provider)
        {
            var dataLockErrors = table.CreateSet<DataLockError>().ToList();

            foreach (var dataLockError in dataLockErrors)
            {
                if (string.IsNullOrWhiteSpace(dataLockError.Apprenticeship)) continue;

                var apprenticeship = Apprenticeships.FirstOrDefault(a => a.Identifier == dataLockError.Apprenticeship) ??
                                     throw new Exception($"Can't find a matching apprenticeship for Identifier {dataLockError.Apprenticeship}");

                dataLockError.ApprenticeshipId = apprenticeship.ApprenticeshipId;

            }

            var matcher = new EarningFailedDataLockMatcher(provider, TestSession, CurrentCollectionPeriod, dataLockErrors);
            await WaitForIt(() => matcher.MatchPayments(), "DataLock Failed event check failure");
        }



    }
}
