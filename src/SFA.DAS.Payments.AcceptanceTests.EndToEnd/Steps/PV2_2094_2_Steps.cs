using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-2094-2-correctly-refund-duplicate-payment-claw-backs-when-a-learner-is-added-back-into-ILR")]
    [Scope(Feature = "PV2-2094-3-correctly-refund-duplicate-payment-claw-backs-when-a-learner-is-added-back-into-ILR")]
    // ReSharper disable once InconsistentNaming
    public class PV2_2094_2_Steps : FM36_ILR_Base_Steps
    {
        public PV2_2094_2_Steps(FeatureContext context) : base(context)
        {
        }

        private Learner learnerA;
        private Learner learnerB;
        private const string PriceEpisodeIdentifierA = "PEA-2094";
        private const string PriceEpisodeIdentifierB = "PEB-2094";
        private const string CommitmentIdentifierA = "A-2094";
        private const string CommitmentIdentifierB = "B-2094";

        private Learner GenerateLearner()
        {
            var newLearner = TestSession.GenerateLearner(TestSession.Provider.Ukprn);
            TestSession.Learners.Add(newLearner);
            return newLearner;
        }

        [Given(@"Commitment exists - which should this match, needs to match the commitments in FM36 in say was as alex spec")]
        public async Task GivenCommitmentExists_WhichShouldThisMatchNeedsToMatchTheCommitmentsInFMInSayWasAsAlexSpec()
        {
            GetFm36LearnerForCollectionPeriod("R12/current academic year");

            learnerA = GenerateLearner();
            learnerB = GenerateLearner();

            await SetupTestCommitmentData(CommitmentIdentifierA, PriceEpisodeIdentifierA, null, null, 9999999999, learnerA);
            await SetupTestCommitmentData(CommitmentIdentifierB, PriceEpisodeIdentifierB, null, null, 8888888888, learnerB);
        }

        [When(@"following provider payments exists in database For period (.*)")]
        public async Task WhenFollowingProviderPaymentsExistsInDatabaseForPeriod(string collectionPeriod, Table table)
        {
            await CreatePaymentModel(table);
        }

        [When(@"an ILR file is submitted for period (.*)")]
        public async Task WhenAnIlrFileIsSubmittedForPeriodR(string collectionPeriod)
        {
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);
        }

        [When(@"After Period-end following provider payments will be generated in database")]
        public async Task WhenAfterPeriod_EndFollowingProviderPaymentsWillBeGeneratedInDatabase(Table table)
        {
            await WaitForRequiredPayments(table.RowCount);

            await EmployerMonthEndHelper.SendLevyMonthEndForEmployers(
                TestSession.GenerateId(),
                TestSession.Employers.Select(x => x.AccountId),
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                MessageSession);

            var expectedPayments = table.CreateSet<ProviderPayment>().ToList();
            expectedPayments.ForEach(ep => ep.Uln = ep.LearnerId == "learner a" ? learnerA.Uln : learnerB.Uln);

            await WaitForIt(() => AssertExpectedPayments(expectedPayments), "Failed to wait for expected number of payments");
        }

        private bool AssertExpectedPayments(List<ProviderPayment> expectedPayments)
        {
            var actualPayments = Scope.Resolve<IPaymentsHelper>().GetPayments(TestSession.Provider.Ukprn, TestSession.CollectionPeriod);

            if (actualPayments.Count != expectedPayments.Count) return false;

            return expectedPayments.All(ep => actualPayments.Any(ap =>
                ep.Uln == ap.LearnerUln &&
                ep.TransactionType == ap.TransactionType &&
                ep.LevyPayments == ap.Amount &&
                ep.ParsedDeliveryPeriod.Period == ap.DeliveryPeriod &&
                ep.ParsedCollectionPeriod.AcademicYear == ap.CollectionPeriod.AcademicYear &&
                ep.ParsedCollectionPeriod.Period == ap.CollectionPeriod.Period
            ));
        }

        private async Task CreatePaymentModel(Table table, bool missingApprenticeshipId = false)
        {
            var payments = table.CreateSet<ProviderPayment>().ToList();
            var jobId = TestSession.GenerateId();
            var paymentHistory = new List<PaymentModel>();

            foreach (var providerPayment in payments)
            {
                var apprenticeship = TestSession.Apprenticeships[providerPayment.LearnerId == "learner a" ? CommitmentIdentifierA : CommitmentIdentifierB];

                var learner = providerPayment.LearnerId == "learner a" ? learnerA : learnerB;

                paymentHistory.Add(new PaymentModel
                {
                    EventId = Guid.NewGuid(),
                    JobId = jobId,
                    CollectionPeriod = providerPayment.ParsedCollectionPeriod,
                    DeliveryPeriod = providerPayment.ParsedDeliveryPeriod.Period,
                    Ukprn = TestSession.Provider.Ukprn,
                    LearnerUln = learner.Uln,
                    LearnerReferenceNumber = learner.LearnRefNumber,
                    SfaContributionPercentage = 0.95m,
                    TransactionType = providerPayment.TransactionType,
                    ContractType = ContractType.Act1,
                    PriceEpisodeIdentifier = providerPayment.LearnerId == "learner a" ? PriceEpisodeIdentifierA : PriceEpisodeIdentifierB,
                    FundingSource = FundingSourceType.Levy,
                    LearningAimPathwayCode = 1,
                    LearningAimReference = "ZPROG001",
                    LearningAimStandardCode = 0,
                    IlrSubmissionDateTime = DateTime.Now,
                    Amount = providerPayment.LevyPayments,
                    LearningAimFundingLineType = "19+ Apprenticeship (Employer on App Service)",
                    LearningAimFrameworkCode = 418,
                    LearningAimProgrammeType = 20,
                    AccountId = apprenticeship?.AccountId,
                    TransferSenderAccountId = apprenticeship?.TransferSendingEmployerAccountId,
                    StartDate = DateTime.UtcNow,
                    PlannedEndDate = DateTime.UtcNow,
                    ActualEndDate = DateTime.UtcNow,
                    CompletionStatus = 1,
                    CompletionAmount = 9000M,
                    InstalmentAmount = 600M,
                    NumberOfInstalments = 12,
                    ReportingAimFundingLineType = "19+ Apprenticeship (Employer on App Service) Levy funding",
                    ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                    LearningStartDate = DateTime.Now,

                    ApprenticeshipId = missingApprenticeshipId ? null : apprenticeship?.Id,
                    ApprenticeshipPriceEpisodeId = missingApprenticeshipId ? null : apprenticeship?.ApprenticeshipPriceEpisodes.First().Id,
                });
            }

            await DataContext.Payment.AddRangeAsync(paymentHistory);
            await DataContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
