using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-1953-small-employer-retrospective")]
    // ReSharper disable once InconsistentNaming
    public class PV2_1953_Steps : FM36_ILR_Base_Steps
    {
        public PV2_1953_Steps(FeatureContext context) : base(context)
        {
        }

        private const string PriceEpisodeIdentifier = "20-593-1-06/08/2020";
        private const string CommitmentIdentifier = "A-1953";

        [Given("a learner submits an ILR when not employed by a small employer in R03")]
        public async Task R03NotSmallEmployer()
        {
            GetFm36LearnerForCollectionPeriod("R03/current academic year");
            await SetupTestCommitmentData(CommitmentIdentifier, PriceEpisodeIdentifier);
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);

            await WaitForPayments(12);
        }
        [When("a learner submits an ILR when employed by a small employer in R04")]
        public async Task R04SmallEmployerFromBeginning()
        {
            GetFm36LearnerForCollectionPeriod("R04/current academic year");
            await SetupTestCommitmentData(CommitmentIdentifier, PriceEpisodeIdentifier);
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);

            await WaitForPayments(14);
        }

        [Then(@"there should be payments refunding and repaying periods 1-3")]
        public async Task ThenThereShouldBeRefundPaymentsForPeriodsOneToThree()
        {
            await WaitForRefunds(6);
        }

        protected async Task WaitForRefunds(int count)
        {
            await WaitForIt(() =>
                {
                    var payments = Scope.Resolve<IPaymentsHelper>()
                               .GetPayments(TestSession.Provider.Ukprn, TestSession.CollectionPeriod);
                    return payments.Count(x => x.Amount < 0) == count;
                },
                "Failed to wait for expected number of payments");
        }
    }
}