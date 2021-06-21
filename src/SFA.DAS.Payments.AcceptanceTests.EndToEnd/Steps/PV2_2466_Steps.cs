using Autofac;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-2466-Levy-Learner-PriceEpisodeAimSeqNumber-IsMapped-FromILR-ToPaymentsTable")]
    [Scope(Feature = "PV2-2466-Non-Levy-Learner-PriceEpisodeAimSeqNumber-IsMapped-FromILR-ToPaymentsTable")]
    public class PV2_2466_Steps : FM36_ILR_Base_Steps
    {
        private const long LearningAimSequenceNumber = 1234;
        private const string PriceEpisodeIdentifier = "PE-2466";
        private const string CommitmentIdentifier = "A-2466";

        public PV2_2466_Steps(FeatureContext context) : base(context)
        {
        }

        [Given(@"a learner funded by a levy paying employer exists")]
        public async Task GivenALearnerFundedByALevyPayingEmployerWithPriceEpisodeAimSeqNumberExists()
        {
            GetFm36LearnerForCollectionPeriod("R03/current academic year");

            await SetupTestCommitmentData(CommitmentIdentifier, PriceEpisodeIdentifier);
        }

        [Given(@"a learner funded by a non levy paying employer exists")]
        public async Task GivenALearnerFundedByANonLevyPayingEmployerExists()
        {
            GetFm36LearnerForCollectionPeriod("R03/current academic year");

            await SetupTestCommitmentData(CommitmentIdentifier, PriceEpisodeIdentifier);
        }

        [When(@"A submission is processed for the levy learner")]
        public async Task WhenASubmissionIsProcessedForthelevyLearner()
        {
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
        }

        [When(@"A submission is processed for the non levy learner")]
        public async Task WhenASubmissionIsProcessedForTheNonLevyLearner()
        {
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);
        }

        [Then(@"PriceEpisodeAimSeqNumber is correctly mapped on each of (.*) payments")]
        public async Task ThenPriceEpisodeAimSeqNumberIsCorrectlyMappedOnEachOfPayments(int paymentCount)
        {
            await WaitForPayments(paymentCount);

            var payments = await DataContext.Payment
                .Where(x =>
                            x.CollectionPeriod.AcademicYear == TestSession.CollectionPeriod.AcademicYear &&
                            x.CollectionPeriod.Period == TestSession.CollectionPeriod.Period &&
                            x.LearnerUln == TestSession.Learner.Uln &&
                            x.Ukprn == TestSession.Ukprn)
                .ToListAsync();

            payments.Count().Should().Be(paymentCount);
            payments
                .Where(x => x.TransactionType != TransactionType.OnProgrammeMathsAndEnglish)
                .ForEach(x => x.LearningAimSequenceNumber.Should().Be(LearningAimSequenceNumber));
        }
    }
}