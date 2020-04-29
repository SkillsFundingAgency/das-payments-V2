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
    [Scope(Feature = "PV2-1849-Levy-Learner-Made-Redundant-Outside-Of-Last-6-Months")]
    // ReSharper disable once InconsistentNaming
    public class PV2_1849_Steps : FM36_ILR_Base_Steps
    {
        public PV2_1849_Steps(FeatureContext context) : base(context)
        {
        }

        private const string PriceEpisodeIdentifier = "PE-1849";
        private const string CommitmentIdentifier = "A-1849";

        [Given("the learner does not find alternative employment")]
        [Given("the ILR submission for the learner contains 'Price episode read status code' not equal to '0'")]
        [Given("the 'Price episode read start date' shows date of redundancy is more than 6mths of planned learning")]
        [When("the submission is processed for payment")]
        public void EmptyIlrSetupStep()
        {
            //NOTE: This is handled by the FM36 we import
        }

        [Given("a learner funded by a levy paying employer is made redundant")]
        public async Task LevyLearnerMadeRedundant()
        {
            ImportR03Fm36ForNonRedundantLevyLearner();

            await SetUpMatchingCommitment();

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);

            await WaitForRequiredPayments(6);

            await EmployerMonthEndHelper.SendLevyMonthEndForEmployers(
                TestSession.GenerateId(),
                TestSession.Employers.Select(x => x.AccountId),
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                MessageSession);

            await WaitForPayments(6);
        }

        [Given("there is more than 6 months remaining of the planned learning")]
        public async Task ThereAreLessThan6MonthsRemainingOfPlannedLearning()
        {
            ImportR07Fm36ToMakeLearnerRedundant();

            await SetUpMatchingCommitment();

            CreateDataLockForCommitment(CommitmentIdentifier);

            TestSession.RegenerateJobId();

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);

            await EmployerMonthEndHelper.SendLevyMonthEndForEmployers(
                TestSession.GenerateId(),
                TestSession.Employers.Select(x => x.AccountId),
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                MessageSession);
        }

        private void ImportR03Fm36ForNonRedundantLevyLearner() { GetFm36LearnerForCollectionPeriod("R03/current academic year"); }

        private void ImportR07Fm36ToMakeLearnerRedundant() { GetFm36LearnerForCollectionPeriod("R07/current academic year"); }

        private async Task SetUpMatchingCommitment() { await SetupTestCommitmentData(CommitmentIdentifier, PriceEpisodeIdentifier); }

        private void CreateDataLockForCommitment(string commitmentIdentifier)
        {
            var context = Scope.Resolve<TestPaymentsDataContext>();
            var apprenticeship = context.Apprenticeship.Single(x => x.Id == TestSession.Apprenticeships[commitmentIdentifier].Id);
            apprenticeship.FrameworkCode += 1;
            context.SaveChanges();
        }

        private bool HasNoDataLocksForPriceEpisodeInR04(string priceEpisodeIdentifier, short academicYear)
        {
            return !EarningEventsHelper.
                GetOnProgrammeDataLockErrorsForLearnerAndPriceEpisodeAndDeliveryPeriod(priceEpisodeIdentifier, academicYear, TestSession, 4)
                .Any();
        }

        private bool HasPayableEarningEventsForPriceEpisode(string priceEpisodeIdentifier)
        {
            return EarningEventsHelper.PayableEarningsReceivedForLearner(TestSession).Any(x => x.PriceEpisodes.Any(y => y.Identifier == priceEpisodeIdentifier));
        }

        private bool HasCorrectlyFundedUpToR07(short academicYear, int fundingSource, int sfaPercentage)
        {
            return FundingSourcePaymentEventsHelper
                .FundingSourcePaymentsReceivedForLearner(PriceEpisodeIdentifier, academicYear, TestSession)
                .Count(x =>
                    x.FundingSourceType == (FundingSourceType)fundingSource
                    && x.SfaContributionPercentage == (decimal)sfaPercentage / 100
                    && x.ContractType == ContractType.Act1
                    && x.AmountDue == 1000m) == 3; //Should be 3 payments for R04,5,6 but not 7
        }

        [Then("bypass the data lock rules")]
        public async Task BypassTheDataLockRules()
        {
            await WaitForIt(() => HasPayableEarningEventsForPriceEpisode(PriceEpisodeIdentifier) && HasNoDataLocksForPriceEpisodeInR04(PriceEpisodeIdentifier, short.Parse(TestSession.FM36Global.Year)),
                "Failed to find a Payable Earning and no Data Locks");
        }

        [Then(@"fund 12 weeks of the learning from Funding Source (.*), with (.*)% SFA funding, from the date of the Price episode read start date")]
        public async Task ThenFundTheRemainingInstallmentsCorrectly(int fundingSource, int sfaPercentage)
        {
            await WaitForIt(() => HasCorrectlyFundedUpToR07(short.Parse(TestSession.FM36Global.Year), fundingSource, sfaPercentage),
                "Failed to find correctly funded remaining installments");
        }

        [Then(@"continue to fund the monthly instalments prior to redundancy date as per existing ACT1 rules \(Funding Source 1\)")]
        public void ThenContinueToFundTheMonthlyInstalmentsPriorToRedundancyDateAsPerExistingAct1RulesFundingSource1()
        {
            //covered by waiting for payments in the first Given statement
        }
    }
}