using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-1845-Levy-Learner-Made-Redundant-In-Last-6-Months")]
    // ReSharper disable once InconsistentNaming
    public class PV2_1845_Steps : FM36_ILR_Base_Steps
    {
        public PV2_1845_Steps(FeatureContext context) : base(context)
        {
        }

        private const string PriceEpisodeIdentifier = "PE-1845";
        private const string CommitmentIdentifier = "A-1845";

        [Given("the learner does not find alternative employment")]
        [Given("the ILR submission for the learner contains 'Price episode read status code' not equal to '0'")]
        [Given("the 'Price episode read start date' shows date of redundancy is within 6mths of planned end date")]
        [When("the submission is processed for payment")]
        public void EmptyIlrSetupStep()
        {
            //NOTE: This is handled by the FM36 we import
        }

        [Given("a learner funded by a levy paying employer is made redundant")]
        public async Task LevyLearnerMadeRedundant()
        {
            ImportR07Fm36ForNonRedundantLevyLearner();
            TestSession.CollectionPeriod.AcademicYear = 1920; //this should be handled by the base steps...

            await SetUpMatchingCommitment();

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);

            await WaitForRequiredPayments(14);

            await EmployerMonthEndHelper.SendLevyMonthEndForEmployers(
                TestSession.GenerateId(),
                TestSession.Employers.Select(x => x.AccountId),
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                MessageSession);

            await WaitForPayments(14);
        }

        [Given("there are less than 6 months remaining of the planned learning")]
        public async Task ThereAreLessThan6MonthsRemainingOfPlannedLearning()
        {
            ImportR12Fm36ToMakeLearnerRedundant();
            TestSession.CollectionPeriod.AcademicYear = 1920; //this should be handled by the base steps...

            await SetUpMatchingCommitment();

            CreateDataLockForCommitment(CommitmentIdentifier);
            TestSession.RegenerateJobId();

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);
        }

        private void ImportR07Fm36ForNonRedundantLevyLearner() { GetFm36LearnerForCollectionPeriod("R07/1920"); }

        private void ImportR12Fm36ToMakeLearnerRedundant() { GetFm36LearnerForCollectionPeriod("R12/1920"); }

        private async Task SetUpMatchingCommitment() { await SetupTestCommitmentData(CommitmentIdentifier, PriceEpisodeIdentifier); }

        private void CreateDataLockForCommitment(string commitmentIdentifier)
        {
            var context = Scope.Resolve<TestPaymentsDataContext>();
            var apprenticeship =
                context.Apprenticeship.Single(x => x.Id == TestSession.Apprenticeships[commitmentIdentifier].Id);
            apprenticeship.FrameworkCode += 1;
            context.SaveChanges();
        }

        private bool HasNoDataLocksForPriceEpisodeInR08(string priceEpisodeIdentifier, short academicYear)
        {
            return !EarningEventsHelper
                .GetOnProgrammeDataLockErrorsForLearnerAndPriceEpisodeAndDeliveryPeriod(priceEpisodeIdentifier, academicYear, TestSession, 8)
                .Any();
        }

        private bool HasPayableEarningEventsForPriceEpisode(string priceEpisodeIdentifier)
        {
            return EarningEventsHelper.PayableEarningsReceivedForLearner(TestSession)
                .Any(x => x.PriceEpisodes.Any(y => y.Identifier == priceEpisodeIdentifier));
         }

        private bool HasCorrectlyFundedFromR08(short academicYear, int fundingSource, int sfaPercentage )
        {
            return FundingSourcePaymentEventsHelper
                .FundingSourcePaymentsReceivedForLearner(PriceEpisodeIdentifier, academicYear, TestSession)
                .Count(x =>
                    x.FundingSourceType == (FundingSourceType) fundingSource
                    && x.SfaContributionPercentage == (decimal) sfaPercentage/100
                    && x.ContractType == ContractType.Act1
                    && x.AmountDue == 1000m) == 5;
        }

        [Then("bypass the data lock rules")]
        public async Task BypassTheDataLockRules()
        {
            await WaitForIt(
                () => HasPayableEarningEventsForPriceEpisode(PriceEpisodeIdentifier) &&
                      HasNoDataLocksForPriceEpisodeInR08(PriceEpisodeIdentifier,
                          short.Parse(TestSession.FM36Global.Year)),
                "Failed to find a Payable Earning and no Data Locks");
        }

        [Then(
            @"fund the remaining monthly instalments of the learning from Funding Source (.*) \((.*)% SFA funding\) from the date of the Price episode read start date")]
        public async Task ThenFundTheRemainingInstallmentsCorrectly(int fundingSource, int sfaPercentage)
        {
            await WaitForIt(() => HasCorrectlyFundedFromR08(short.Parse(TestSession.FM36Global.Year), fundingSource, sfaPercentage),
                "Failed to find correctly funded remaining installments");
        }

        [Then(@"continue to fund the monthly instalments prior to redundancy date as per existing ACT1 rules \(Funding Source 1\)")]
        public void ThenContinueToFundTheMonthlyInstalmentsPriorToRedundancyDateAsPerExistingAct1RulesFundingSource1()
        {
            //covered by waiting for payments in the first Given statement
        }
    }
}