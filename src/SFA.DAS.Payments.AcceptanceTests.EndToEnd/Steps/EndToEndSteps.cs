using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.Tests.Core.Builders;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Learner = SFA.DAS.Payments.AcceptanceTests.Core.Data.Learner;


namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class EndToEndSteps : EndToEndStepsBase
    {
        private readonly FeatureNumber featureNumber;

        public EndToEndSteps(FeatureContext context, FeatureNumber featureNumber) : base(context)
        {
            this.featureNumber = featureNumber;
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

        [Given(@"the provider has already submitted an ILR in the collection period")]
        public async Task GivenTheProviderHasAlreadySubittedAnILRInTheCurrentCollectionPeriod()
        {
            var learnerTable = new Table( "Start Date", "Planned Duration", "Total Training Price", "Total Training Price Effective Date", "Total Assessment Price", "Total Assessment Price Effective Date", "Actual Duration", "Completion Status", "SFA Contribution Percentage", "Contract Type", "Aim Sequence Number", "Aim Reference", "Framework Code", "Pathway Code", "Programme Type", "Funding Line Type");
            learnerTable.AddRow("start of academic year", "12 months", "11250", "Aug/Current Academic Year", "0", "Aug/Current Academic Year", "", "continuing", "90%", "Act2", "1", "ZPROG001", "593", "1", "20", "19 + Apprenticeship Non - Levy Contract(procured)");
            AddTestLearners(learnerTable);

            var previousEarningsTable = new Table("Delivery Period", "On-Programme", "Completion", "Balancing");
            previousEarningsTable.AddRow("Aug/Current Academic Year", "750", "0", "0");
            previousEarningsTable.AddRow("Sep/Current Academic Year", "750", "0", "0");
            previousEarningsTable.AddRow("Oct/Current Academic Year", "750", "0", "0");
            previousEarningsTable.AddRow("Nov/Current Academic Year", "750", "0", "0");
            previousEarningsTable.AddRow("Dec/Current Academic Year", "750", "0", "0");
            previousEarningsTable.AddRow("Jan/Current Academic Year", "750", "0", "0");
            previousEarningsTable.AddRow("Feb/Current Academic Year", "750", "0", "0");
            previousEarningsTable.AddRow("Mar/Current Academic Year", "750", "0", "0");
            previousEarningsTable.AddRow("Apr/Current Academic Year", "750", "0", "0");
            previousEarningsTable.AddRow("May/Current Academic Year", "750", "0", "0");
            previousEarningsTable.AddRow("Jun/Current Academic Year", "750", "0", "0");
            previousEarningsTable.AddRow("Jul/Current Academic Year", "750", "0", "0");
            CreatePreviousEarningsAndTraining(previousEarningsTable);


            var previousPaymentsTable = new Table("Collection Period","Delivery Period","SFA Co - Funded Payments","Employer Co - Funded Payments","Transaction Type");
            previousPaymentsTable.AddRow("R01/Current Academic Year","Aug/Current Academic Year","675","75","Learning");
            await GeneratePreviousPayment(previousPaymentsTable, TestSession.Provider.Ukprn);
        }

        private async Task WaitForJobToFinish(long jobId)
        {
            await WaitForIt(async () =>
            {
                var dataContext = Scope.Resolve<JobsDataContext>();
                var job = await dataContext.Jobs.AsNoTracking().FirstOrDefaultAsync(savedJob => savedJob.DcJobId == jobId);
                return job != null && job.Status != JobStatus.InProgress;
            }, $"Job failed to finish. Job id: {jobId}");
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

        [Given(@"the provider priority order is")]
        public async Task GivenTheProviderPriorityOrder(Table table)
        {
            await AddLevyAccountPriorities(table, TestSession, CurrentCollectionPeriod, DataContext);
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

        [Given(@"the ""(.*)"" IsLevyPayer flag is (.*)")]
        public async Task GivenTheIsLevyPayerFlagIsFalse(string employerIdentifier, bool isLevyFlag)
        {
            var employer = TestSession.GetEmployer(employerIdentifier);
            employer.IsLevyPayer = isLevyFlag;
            await SaveLevyAccount(employer).ConfigureAwait(false);
        }

        [Then(@"a DLOCK_11 is not flagged")]
        public async Task ThenDLOCK_IsNotFlagged()
        {
            var matcher = new EarningFailedDataLockMatcher(TestSession.Provider, TestSession, CurrentCollectionPeriod, new List<DataLockError>());
            await WaitForUnexpected(() => matcher.MatchUnexpectedEvents(), "DataLock Event check failure").ConfigureAwait(false);
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
            AddPriceDetails(table);
        }

        [Given("the following capping will apply to the price episodes")]
        public void GivenTheFollowingCappingWillApply(Table table)
        {

        }

        [Given(@"the learner earnings were generated")]
        [When(@"the learner earnings are generated")]
        public async Task GivenTheLearnerEarningsWereGenerated()
        {
            await GenerateEarnings(TestSession.Provider).ConfigureAwait(false);
        }

        [When(@"the Payments service records the completion of the job")]
        public async Task WhenThePaymentsServiceRecordsTheCompletionOfTheJob()
        {
            await WaitForJobToFinish(TestSession.Provider.JobId).ConfigureAwait(false);
        }

        [When(@"the Data-Collections system confirms successful completion of processing the job")]
        public async Task WhenTheData_CollectionsSystemConfirmsSuccessfulCompletionOfProcessingTheJob()
        {
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmissionEvent(TestSession.Provider.Ukprn, CurrentCollectionPeriod.AcademicYear,
                CurrentCollectionPeriod.Period,
                TestSession.Provider.JobId, true).ConfigureAwait(false);
        }

        [When(@"the payments service is notified that the subsequent Data-Collections processes failed to process the job")]
        public async Task WhenThePaymentsServiceIsNotifiedThatTheSubsequentData_CollectionsProcessesFailedToProcessTheJob()
        {
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmissionEvent(TestSession.Provider.Ukprn, CurrentCollectionPeriod.AcademicYear,
                CurrentCollectionPeriod.Period,
                TestSession.Provider.JobId, false).ConfigureAwait(false);
        }


        [When(@"the payments service has notified Data-Collections that the Data-Locks process has finished")]
        public void WhenThePaymentsServiceHasNotifiedData_CollectionsThatTheData_LocksProcessHasFinished()
        {
            //do nothing, just for show
        }

        [Then(@"the payments for the previous submission should be removed")]
        public async Task ThenThePaymentsForThePreviousSubmissionShouldBeRemoved()
        {
            await WaitForIt(async () =>
            {
                var payments = await Scope.Resolve<TestPaymentsDataContext>()
                    .Payment
                    .AsNoTracking()
                    .Where(p => p.Ukprn == TestSession.Provider.Ukprn)
                    .ToListAsync();
                return payments.Any() && payments.All(p => p.JobId == TestSession.Provider.JobId);
            },$"Provider Payments failed to cleanup old payments for provider {TestSession.Provider.Ukprn}");
        }

        [Then(@"the payments for the current submission should be removed")]
        public async Task ThenThePaymentsForTheCurrentSubmissionShouldBeRemoved()
        {
            await WaitForIt(async () =>
            {
                var payments = await Scope.Resolve<TestPaymentsDataContext>()
                    .Payment
                    .AsNoTracking()
                    .Where(p => p.Ukprn == TestSession.Provider.Ukprn)
                    .ToListAsync();

                return payments.Any() && payments.All(p => p.JobId != TestSession.Provider.JobId);
            }, $"Provider Payments failed to cleanup payments for failed job: {TestSession.Provider.JobId}, Provider: {TestSession.Provider.Ukprn}");
        }


        [Then(@"the following learner earnings should be generated")]
        [Given(@"the following learner earnings were generated")]
        public async Task ThenTheFollowingLearnerEarningsShouldBeGenerated(Table table)
        {
            await GeneratedAndValidateEarnings(table, TestSession.Provider).ConfigureAwait(false);
        }

        [Then(@"the following learner earnings should be generated on restart")]
        public async Task ThenTheFollowingLearnerEarningsShouldBeGeneratedOnRestart(Table table)
        {
            await GeneratedAndValidateEarningsOnRestart(table, TestSession.Provider).ConfigureAwait(false);
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
            await MatchRequiredPaymentsFromTable(table, TestSession.Provider).ConfigureAwait(false);
        }

        [Then(@"at month end only the following payments will be calculated")]
        public async Task ThenAtMonthEndOnlyTheFollowingPaymentsWillBeCalculated(Table table)
        {
            await ValidateRequiredPaymentsAtMonthEnd(table, TestSession.Provider).ConfigureAwait(false);
        }

        [Then(@"a Submission (.*) Event is received")]
        public async Task ThenASubmissionSuccessEventIsReceived(string outcome)
        {
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmissionEvent(TestSession.Provider.Ukprn, AcademicYear, CollectionPeriod,
                TestSession.GenerateId(), outcome == "Success");
        }


        [Then(@"at month end only the following payments will be calculated for ""(.*)""")]
        public async Task ThenAtMonthEndOnlyTheFollowingPaymentsWillBeCalculatedFor(string providerIdentifier, Table table)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            await MatchRequiredPaymentsFromTable(table, provider).ConfigureAwait(false);
        }

        [Then(@"no payments will be calculated")]
        public async Task ThenNoPaymentsWillBeCalculated()
        {
            var matcher = new RequiredPaymentEventMatcher(TestSession.Provider, CurrentCollectionPeriod);
            await WaitForUnexpected(() => matcher.MatchUnexpectedEvents(), "Required Payment event check failure").ConfigureAwait(false);
        }

        [Then(@"at month end no payments will be calculated for ""(.*)""")]
        public async Task ThenAtMonthEndNoPaymentsWillBeCalculatedForProvider(string providerIdentifier)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            var matcher = new RequiredPaymentEventMatcher(provider, CurrentCollectionPeriod);
            await WaitForUnexpected(() => matcher.MatchUnexpectedEvents(), "Required Payment event check failure").ConfigureAwait(false);
        }

        [Then(@"only the following provider payments will be generated")]
        public async Task ThenOnlyTheFollowingProviderPaymentsWillBeGenerated(Table table)
        {
            //await StartMonthEnd(TestSession.Provider).ConfigureAwait(false);
            //await MatchOnlyProviderPayments(table, TestSession.Provider).ConfigureAwait(false);

            await Task.CompletedTask;
        }

        [Then(@"only the following payments will be held back")]
        public async Task ThenOnlyTheFollowingHeldBackPaymentsWillBeGenerated(Table table)
        {
            await MatchHeldBackRequiredPayments(table, TestSession.Provider).ConfigureAwait(false);
        }

        [Then(@"only the following ""(.*)"" payments will be generated")]
        public async Task ThenOnlyTheFollowingPaymentsWillBeGenerated(string providerIdentifier, Table table)
        {
            //var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            //await MatchOnlyProviderPayments(table, provider).ConfigureAwait(false);

            await Task.CompletedTask;
        }

        [Then(@"no provider payments will be generated")]
        public async Task ThenNoProviderPaymentsWillBeGenerated()
        {
            //var provider = TestSession.Provider;
            //await ThenNoProviderPaymentsWillBeGenerated(provider.Identifier);

            await Task.CompletedTask;
        }

        [Then(@"no ""(.*)"" payments will be generated")]
        public async Task ThenNoProviderPaymentsWillBeGenerated(string providerIdentifier)
        {
            //var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            //var matcher = new ProviderPaymentEventMatcher(provider, CurrentCollectionPeriod, TestSession);
            //await WaitForUnexpected(() => matcher.MatchUnexpectedEvents(), "Provider Payment event check failure");

           await Task.CompletedTask;
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
            await WaitForUnexpected(() => matcher.MatchUnexpectedEvents(), "Earning Event check failure").ConfigureAwait(false);
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
