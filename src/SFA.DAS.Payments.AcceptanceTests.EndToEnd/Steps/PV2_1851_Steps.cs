using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-1851-Non-Levy-Learner-Made-Redundant-Outside-Of-Last-6-Months")]
    // ReSharper disable once InconsistentNaming
    public class PV2_1851_Steps : FM36_ILR_Base_Steps
    {
        public PV2_1851_Steps(FeatureContext context) : base(context)
        {
        }

        private const string PriceEpisodeIdentifier = "PE-1851";
        private const string CommitmentIdentifier = "A-1851";

        [Given("the learner does not find alternative employment")]
        [Given("the ILR submission for the learner contains 'Price episode read status code' not equal to '0'")]
        [Given("the 'Price episode read start date' shows date of redundancy is more than 6mths of planned learning")]
        [When("the submission is processed for payment")]
        [Then("continue to fund the monthly instalments prior to redundancy date as per existing ACT2 rules, Funding Source 2 (95%) and Funding Source 3 (5%)")]
        public void EmptyIlrSetupStep()
        {
            //NOTE: This is handled by the FM36 we import
        }

        [Given("a learner co-funded by a non-levy paying employer is made redundant")]
        public async Task NonLevyLearnerMadeRedundant()
        {
            GetFm36LearnerForCollectionPeriod("R03/current academic year");
            await SetupTestData(PriceEpisodeIdentifier, null, CommitmentIdentifier, null);
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);

            await WaitForPayments(9);
        }

        [Given("there is more than 6 months remaining of the planned learning")]
        public async Task ThereAreLessThan6MonthsRemainingOfPlannedLearning()
        {
            GetFm36LearnerForCollectionPeriod("R04/current academic year");
            await SetupTestData(PriceEpisodeIdentifier, null, CommitmentIdentifier, null);

            TestSession.RegenerateJobId();

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);
        }

        private bool HasCorrectlyFundedR04(short academicYear)
        {
            return FundingSourcePaymentEventsHelper
                .FundingSourcePaymentsReceivedForLearner(PriceEpisodeIdentifier, academicYear, TestSession)
                .Count(x =>
                    x.FundingSourceType == FundingSourceType.CoInvestedSfa
                    && x.SfaContributionPercentage == 1.0m
                    && x.ContractType == ContractType.Act2
                    && x.AmountDue == 1000m) == 1;
        }

        [Then(@"fund 12 weeks of the learning from Funding Source 2, with 100% SFA funding, from the date of the 'Price episode read start date'")]
        public async Task ThenFundTheRemainingInstallmentsCorrectly()
        {
            await WaitForIt(() => HasCorrectlyFundedR04(short.Parse(TestSession.FM36Global.Year)),
                "Failed to find correctly funded remaining installments");
        }
    }
}