using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore.Internal;
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
        [Then(@"continue to fund the monthly instalments prior to redundancy date as per existing ACT1 rules \(Funding Source 1\)")]
        public void EmptyIlrSetupStep()
        {
            //NOTE: This is handled by the FM36 we import
        }

        [Given("a learner funded by a levy paying employer is made redundant")]
        public async Task LevyLearnerMadeRedundant()
        {
            //submit R03
            GetFm36LearnerForCollectionPeriod("R03/current academic year");
            await SetupTestData(PriceEpisodeIdentifier, null, CommitmentIdentifier, null);
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);

            await WaitForRequiredPayments(6);

            await dcHelper.SendLevyMonthEndForEmployers(
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
            GetFm36LearnerForCollectionPeriod("R04/current academic year");
            await SetupTestData(PriceEpisodeIdentifier, null, CommitmentIdentifier, null, true);

            CreateDataLockForCommitment(CommitmentIdentifier);
            TestSession.RegenerateJobId();

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);

            await dcHelper.SendLevyMonthEndForEmployers(
                TestSession.GenerateId(),
                TestSession.Employers.Select(x => x.AccountId),
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                MessageSession);
        }

        private void CreateDataLockForCommitment(string commitmentIdentifier)
        {
            var context = Scope.Resolve<TestPaymentsDataContext>();
            var apprenticeship = context.Apprenticeship.Single(x => x.Id == TestSession.Apprenticeships[commitmentIdentifier].Id);
            apprenticeship.FrameworkCode += 1;
            context.SaveChanges();
        }

        private bool HasNoDataLocksForPriceEpisodeInR04(string priceEpisodeIdentifier, short academicYear)
        {
            var result = !EnumerableExtensions.Any(EarningEventsHelper
                    .GetOnProgrammeDataLockErrorsForLearnerAndPriceEpisodeAndDeliveryPeriod(priceEpisodeIdentifier, academicYear, TestSession, 4));
            return result;
        }

        private bool HasPayableEarningEventsForPriceEpisode(string priceEpisodeIdentifier)
        {
            return EarningEventsHelper.PayableEarningsReceivedForLearner(TestSession).Any(x => x.PriceEpisodes.Any(y => y.Identifier == priceEpisodeIdentifier));
        }

        private bool HasCorrectlyFundedR04(short academicYear)
        {
            return FundingSourcePaymentEventsHelper
                .FundingSourcePaymentsReceivedForLearner(PriceEpisodeIdentifier, academicYear, TestSession)
                .Count(x =>
                    x.FundingSourceType == FundingSourceType.CoInvestedSfa
                    && x.SfaContributionPercentage == 1.0m
                    && x.ContractType == ContractType.Act1
                    && x.AmountDue == 1000m) == 1;
        }

        [Then("bypass the data lock rules")]
        public async Task BypassTheDataLockRules()
        {
            await WaitForIt(() => HasPayableEarningEventsForPriceEpisode(PriceEpisodeIdentifier) && HasNoDataLocksForPriceEpisodeInR04(PriceEpisodeIdentifier, short.Parse(TestSession.FM36Global.Year)),
                "Failed to find a Payable Earning and no Data Locks");
        }

        [Then(@"fund the remaining monthly instalments of the learning from Funding Source (.*) \((.*)% SFA funding\) from the date of the Price episode read start date")]
        public async Task ThenFundTheRemainingInstallmentsCorrectly(int fundingSource, int sfaPercentage)
        {
            await WaitForIt(() => HasCorrectlyFundedR04(short.Parse(TestSession.FM36Global.Year)),
                "Failed to find correctly funded remaining installments");
        }
    }
}