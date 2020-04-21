using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            GetFm36LearnerForCollectionPeriod("R07/current academic year");
            await SetupTestData(priceEpisodeIdentifier, null, commitmentIdentifier, null);
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);

            await WaitForRequiredPayments(6);

            //await dcHelper.SendPeriodEndTask(20, 3, TestSession.Provider.JobId, "PeriodEndRun");
            await dcHelper.SendLevyMonthEndForEmployers(
                TestSession.GenerateId(),
                TestSession.Employers.Select(x => x.AccountId),
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                MessageSession);

            await WaitForPayments(6);
        }

        [Given("there are less than 6 months remaining of the planned learning")]
        public async Task ThereAreLessThan6MonthsRemainingOfPlannedLearning()
        {
            GetFm36LearnerForCollectionPeriod("R08/current academic year");
            await SetupTestData(priceEpisodeIdentifier, null, commitmentIdentifier, null, true);
            //await SetupTestData(priceEpisodeIdentifier, null, commitmentIdentifier, null);

            CreateDataLockForCommitment(commitmentIdentifier);
            TestSession.RegenerateJobId();

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);

            //await dcHelper.SendPeriodEndTask(20, 4, TestSession.Provider.JobId, "PeriodEndRun");
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
            var apprenticeship =
                context.Apprenticeship.Single(x => x.Id == TestSession.Apprenticeships[commitmentIdentifier].Id);
            apprenticeship.FrameworkCode += 1;
            context.SaveChanges();
        }

        private bool HasNoDataLocksForPriceEpisodeInR04(string priceEpisodeIdentifier, short academicYear)
        {
            var dataLocks =
                EarningEventsHelper.GetOnProgrammeDataLockErrorsForLearnerAndPriceEpisodeAndDeliveryPeriod(
                    priceEpisodeIdentifier, academicYear, TestSession, 4); //todo take this out
            var payableEarningEvents = EarningEventsHelper.PayableEarningsReceivedForLearner(TestSession);

            var result = !EnumerableExtensions.Any(EarningEventsHelper
                .GetOnProgrammeDataLockErrorsForLearnerAndPriceEpisodeAndDeliveryPeriod(priceEpisodeIdentifier,
                    academicYear, TestSession, 4));
            return result;
        }

        private bool HasPayableEarningEventsForPriceEpisode(string priceEpisodeIdentifier)
        {
            return EarningEventsHelper.PayableEarningsReceivedForLearner(TestSession)
                .Any(x => x.PriceEpisodes.Any(y => y.Identifier == priceEpisodeIdentifier));
            //var result = EarningEventsHelper.PayableEarningsReceivedForLearner(TestSession).Count(x => x.PriceEpisodes.Any(y => y.Identifier == priceEpisodeIdentifier)) == 2;
            //return result;
        }

        private bool HasCorrectlyFundedR04(short academicYear)
        {
            //var events = PaymentEventsHelper.ProviderPaymentsReceivedForLearner(priceEpisodeIdentifier, academicYear, TestSession);
            var events =
                FundingSourcePaymentEventsHelper.FundingSourcePaymentsReceivedForLearner(priceEpisodeIdentifier,
                    academicYear, TestSession);

            var filter1 = events.Where(x => x.FundingSourceType == FundingSourceType.CoInvestedSfa);
            //var filter2 = 

            var count = FundingSourcePaymentEventsHelper
                .FundingSourcePaymentsReceivedForLearner(priceEpisodeIdentifier, academicYear, TestSession)
                .Count(x =>
                    x.FundingSourceType == FundingSourceType.CoInvestedSfa
                    && x.SfaContributionPercentage == 1.0m
                    && x.ContractType == ContractType.Act1
                    && x.AmountDue == 1000m);
            return count == 1;
        }

        [Then("bypass the data lock rules")]
        public async Task BypassTheDataLockRules()
        {
            await WaitForIt(
                () => HasPayableEarningEventsForPriceEpisode(priceEpisodeIdentifier) &&
                      HasNoDataLocksForPriceEpisodeInR04(priceEpisodeIdentifier,
                          short.Parse(TestSession.FM36Global.Year)),
                "Failed to find a Payable Earning and no Data Locks");
        }

        [Then(
            @"fund the remaining monthly instalments of the learning from Funding Source (.*) \((.*)% SFA funding\) from the date of the Price episode read start date")]
        public async Task ThenFundTheRemainingInstallmentsCorrectly(int fundingSource, int sfaPercentage)
        {
            await WaitForIt(() => HasCorrectlyFundedR04(short.Parse(TestSession.FM36Global.Year)),
                "Failed to find correctly funded remaining installments");
        }
    }
}