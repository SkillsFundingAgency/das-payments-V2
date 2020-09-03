using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-2094-Prevent-duplicate-payment-claw-backs-when-a-learner-is-deleted-from-the-ILR")]
    // ReSharper disable once InconsistentNaming
    public class PV2_2094_Steps : FM36_ILR_Base_Steps
    {
        public PV2_2094_Steps(FeatureContext context) : base(context)
        {
        }

        private Learner learnerA;
        private Learner learnerB;
        private const string PriceEpisodeIdentifierA = "PEA-2094";
        private const string PriceEpisodeIdentifierB = "PEB-2094";
        private const string CommitmentIdentifierA = "A-2094";
        private const string CommitmentIdentifierB = "B-2094";

        [Given("Commitment exists - which should this match, needs to match the commitments in FM36 in say was as alex spec")]
        public async Task EmptyIlrSetupStep()
        {
            GetFm36LearnerForCollectionPeriod("R05/current academic year");

            learnerA = GenerateLearner();
            learnerB = GenerateLearner();

            await SetupTestCommitmentData(CommitmentIdentifierA, PriceEpisodeIdentifierA, null, null, 9999999999, learnerA);
            await SetupTestCommitmentData(CommitmentIdentifierB, PriceEpisodeIdentifierB, null, null, 8888888888, learnerB);

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);
        }

        [When("an ILR file is submitted for period (.*)")]
        public async Task LevyLearnerMadeRedundant(string collectionPeriod)
        {
            if (collectionPeriod == "R05") return;

            GetFm36LearnerForCollectionPeriod($"{collectionPeriod}/current academic year");

            ResetFm36LearnerDetails();

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);
        }

        private Learner GenerateLearner()
        {
            var newLearner = TestSession.GenerateLearner(TestSession.Provider.Ukprn);
            TestSession.Learners.Add(newLearner);
            return newLearner;
        }

        private void ResetFm36LearnerDetails()
        {
            var fm36LearnerA = TestSession.FM36Global.Learners.SingleOrDefault(l => l.ULN == 9999999999);
            if (fm36LearnerA != null)
            {
                fm36LearnerA.ULN = learnerA.Uln;
                fm36LearnerA.LearnRefNumber = learnerA.LearnRefNumber;
            }

            var fm36LearnerB = TestSession.FM36Global.Learners.SingleOrDefault(l => l.ULN == 8888888888);
            if (fm36LearnerB != null)
            {
                fm36LearnerB.ULN = learnerB.Uln;
                fm36LearnerB.LearnRefNumber = learnerB.LearnRefNumber;
            }
        }

        [When("After Period-end following provider payments will be generated in database")]
        [Then("After Period-end following provider payments will be generated in database")]
        public async Task AfterPeriodEndRunPaymentsAreGenerated(Table table)
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
            var actualPayments= Scope.Resolve<IPaymentsHelper>().GetPayments(TestSession.Provider.Ukprn, TestSession.CollectionPeriod);
            
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
    }
}