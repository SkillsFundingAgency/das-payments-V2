using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-2466-Levy-Learner-PriceEpisodeAimSeqNumber-IsMapped-FromILR-ToPaymentsTable")]
    public class PV2_2466_Steps: FM36_ILR_Base_Steps
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

            //TestSession.FM36Global.Learners.Single().PriceEpisodes.ForEach(x => x.PriceEpisodeValues.PriceEpisodeAimSeqNumber = priceEpisodeAimSeqNumber);
            //TestSession.FM36Global.Learners.Single().LearningDeliveries.ForEach(x => x.AimSeqNumber = priceEpisodeAimSeqNumber);
        }

        [When(@"A submission is processed for this learner")]
        public async Task WhenASubmissionIsProcessedForThisLearner()
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

        [Then(@"PriceEpisodeAimSeqNumber is correctly mapped on each payment")]
        public async Task ThenPriceEpisodeAimSeqNumberIsCorrectlyMappedOnEachPayment()
        {
            await WaitForPayments(6);

            var payments = await DataContext.Payment
                .Where(x => 
                            x.CollectionPeriod.AcademicYear == TestSession.CollectionPeriod.AcademicYear &&
                            x.CollectionPeriod.Period == TestSession.CollectionPeriod.Period &&
                            x.LearnerUln == TestSession.Learner.Uln &&
                            x.Ukprn == TestSession.Ukprn)
                .ToListAsync();

            payments.Count().Should().Be(6);
            payments
                .Where(x => x.TransactionType != TransactionType.OnProgrammeMathsAndEnglish)
                .ForEach(x => x.LearningAimSequenceNumber.Should().Be(LearningAimSequenceNumber));
        }
    }
}
