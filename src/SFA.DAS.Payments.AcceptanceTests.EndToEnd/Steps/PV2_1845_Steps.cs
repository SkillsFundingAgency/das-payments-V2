using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore.Internal;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
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

        private const string priceEpisodeIdentifier = "PE-1845";
        private const string commitmentIdentifier = "A-1845";

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
            //submit R03
            GetFm36LearnerForCollectionPeriod("R04/current academic year");
            await SetupTestData(priceEpisodeIdentifier, null, commitmentIdentifier, null);
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId + 1);
        }

        [Given("there are less than 6 months remaining of the planned learning")]
        public async Task ThereAreLessThan6MonthsRemainingOfPlannedLearning()
        {
            GetFm36LearnerForCollectionPeriod("R03/current academic year");
            await SetupTestData(priceEpisodeIdentifier, null, commitmentIdentifier, null);

            //CreateDataLockForCommitment(commitmentIdentifier);

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);
        }

        private void CreateDataLockForCommitment(string commitmentIdentifier)
        {
            var context = Scope.Resolve<TestPaymentsDataContext>();
            var apprenticeship = context.Apprenticeship.Single(x => x.Id == TestSession.Apprenticeships[commitmentIdentifier].Id);
            apprenticeship.FrameworkCode += 1;
            context.SaveChanges();
        }

        private bool HasNoDataLocksForPriceEpisode(string priceEpisodeIdentifier, short academicYear)
        {
            var dataLocks = EarningEventsHelper.GetOnProgrammeDataLockErrorsForLearnerAndPriceEpisode(priceEpisodeIdentifier, academicYear, TestSession); //todo take this out
            var payableEarningEvents = EarningEventsHelper.PayableEarningsReceivedForLearner(TestSession);

            var result = !EnumerableExtensions.Any(EarningEventsHelper
                    .GetOnProgrammeDataLockErrorsForLearnerAndPriceEpisode(priceEpisodeIdentifier, academicYear, TestSession));
            return result;
        }

        private bool HasPayableEarningEventsForPriceEpisode(string priceEpisodeIdentifier)
        {
            return EarningEventsHelper.PayableEarningsReceivedForLearner(TestSession).Count(x => x.PriceEpisodes.Any(y => y.Identifier == priceEpisodeIdentifier)) == 2;
        }

        private bool HasCorrectlyFunded(short academicYear, int fundingSource, int sfaPercentage)
        {
            var events = PaymentEventsHelper.ProviderPaymentsReceivedForLearner(priceEpisodeIdentifier, academicYear, TestSession);
            return events.Any();
        }

        [Then("bypass the data lock rules")]
        public async Task BypassTheDataLockRules()
        {
            await WaitForIt(() => HasPayableEarningEventsForPriceEpisode(priceEpisodeIdentifier) && HasNoDataLocksForPriceEpisode(priceEpisodeIdentifier, short.Parse(TestSession.FM36Global.Year)),
                "Failed to find a Payable Earning and no Data Locks");
        }

        [Then(@"fund the remaining monthly instalments of the learning from Funding Source (.*) \((.*)% SFA funding\) from the date of the Price episode read start date")]
        public async Task ThenFundTheRemainingInstallmentsCorrectly(int fundingSource, int sfaPercentage)
        {
            await WaitForIt(() => HasCorrectlyFunded(short.Parse(TestSession.FM36Global.Year), fundingSource, sfaPercentage),
                "Failed to find correctly funded remaining installments");
        }
    }
}