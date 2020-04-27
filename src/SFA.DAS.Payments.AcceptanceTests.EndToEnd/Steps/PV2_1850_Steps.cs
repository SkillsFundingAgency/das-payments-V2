using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-1850-Non-Levy-Learner-Made-Redundant-In-Last-6-Months")]
    // ReSharper disable once InconsistentNaming
    public class PV2_1850_Steps : FM36_ILR_Base_Steps
    {
        public PV2_1850_Steps(FeatureContext context) : base(context)
        {
        }

        private const string PriceEpisodeIdentifier = "PE-1850";
        private const string CommitmentIdentifier = "A-1850";

        [Given("the learner does not find alternative employment")]
        [Given("the ILR submission for the learner contains 'Price episode read status code' not equal to '0'")]
        [Given("the 'Price episode read start date' shows date of redundancy is within 6mths of planned end date")]
        [When(@"the Provider submission is processed for payment")]
        public void EmptyIlrSetupStep()
        {
            //NOTE: This is handled by the FM36 we import
        }

        [Given("a learner funded by a non levy paying employer is made redundant")]
        public async Task LevyLearnerMadeRedundant()
        {
            ImportR07Fm36ForNonRedundantNonLevyLearner();

            await SetUpMatchingCommitment();

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);

            await WaitForPayments(21);
        }

        [Given("there are less than 6 months remaining of the planned learning")]
        public async Task ThereAreLessThan6MonthsRemainingOfPlannedLearning()
        {
            ImportR12Fm36ToMakeLearnerRedundant();

            await SetUpMatchingCommitment();

            TestSession.RegenerateJobId();

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);
        }

        private void ImportR07Fm36ForNonRedundantNonLevyLearner() { GetFm36LearnerForCollectionPeriod("R07/current academic year"); }

        private void ImportR12Fm36ToMakeLearnerRedundant() { GetFm36LearnerForCollectionPeriod("R12/current academic year"); }

        private async Task SetUpMatchingCommitment() { await SetupTestCommitmentData(CommitmentIdentifier, PriceEpisodeIdentifier); }

        private bool HasCorrectlyFundedR08Onwards(short academicYear, int fundingSource, int sfaPercentage)
        {
            return FundingSourcePaymentEventsHelper
                .FundingSourcePaymentsReceivedForLearner(PriceEpisodeIdentifier, academicYear, TestSession)
                .Count(x =>
                    x.FundingSourceType == (FundingSourceType)fundingSource
                    && x.SfaContributionPercentage == (decimal)sfaPercentage / 100
                    && x.ContractType == ContractType.Act2
                    && x.AmountDue == 1000m) == 5;
        }

       
        [Then(@"fund the remaining monthly instalments of the learning from Funding Source (.*) \((.*)% SFA funding\) from the date of the Price episode read start date")]
        public async Task ThenFundTheRemainingInstallmentsCorrectly(int fundingSource, int sfaPercentage)
        {
            await WaitForIt(() => HasCorrectlyFundedR08Onwards(short.Parse(TestSession.FM36Global.Year), fundingSource, sfaPercentage),
                "Failed to find correctly funded remaining installments");
        }

        [Then(@"continue to fund the monthly instalments prior to redundancy date as per existing ACT2 rules, Funding Source 2 \(95%\) and Funding Source 3 \(5%\)")]
        public void ThenContinueToFundTheMonthlyInstalmentsPriorToRedundancyDateAsPerExistingAct2RulesFundingSources()
        {
            //covered by waiting for payments in the first Given statement
        }
    }
}