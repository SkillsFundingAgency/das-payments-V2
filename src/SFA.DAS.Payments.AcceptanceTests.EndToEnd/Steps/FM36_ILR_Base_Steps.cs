using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core;
using SFA.DAS.Payments.Tests.Core.Builders;
using TechTalk.SpecFlow;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    // ReSharper disable once InconsistentNaming
    public class FM36_ILR_Base_Steps : EndToEndStepsBase, IDisposable
    {
        private readonly FeatureContext featureContext;
        private TestPaymentsDataContext testDataContext;

        [BeforeStep]
        public void InitialiseNewTestDataContext()
        {
            testDataContext = Scope.Resolve<TestPaymentsDataContext>();
        }

        [AfterStep]
        public void DeScopeTestDataContext()
        {
            testDataContext = null;
        }

        public FM36_ILR_Base_Steps(FeatureContext context) : base(context)
        {
            featureContext = context;
        }

        protected void GetFm36LearnerForCollectionPeriod(string collectionPeriodText)
        {
            TestSession.CollectionPeriod = new CollectionPeriodBuilder().WithSpecDate(collectionPeriodText).Build();
            TestSession.FM36Global = FM36GlobalDeserialiser.DeserialiseByFeatureForPeriod(featureContext.FeatureInfo.Title, TestSession.CollectionPeriod.Period.ToPeriodText());
        }

        [Given(@"there are 2 Commitments in DAS, Commitment A and Commitment B in collection period (.*)")]
        public void GivenThereAreCommitmentsInDasCommitmentAAndCommitmentBInCollectionPeriodRCurrentAcademicYear(string collectionPeriodText)
        {
            GetFm36LearnerForCollectionPeriod(collectionPeriodText);
        }

        [Given(@"there is a single price episode in the ILR, (.*)")]
        public void GivenThereIsASinglePriceEpisodeInTheIlrpe(string p0)
        {
        }

        [Given(@"the start date in the (.*) is before the start date for Commitment (.*)")]
        public async Task GivenTheStartDateInThePeIsBeforeTheStartDateForCommitmentB(string priceEpisodeIdentifier, string commitmentIdentifier)
        {
            bool StartDateIsBefore(DateTime actualPriceEpisodeStartDate, DateTime estimatedStartDate) => actualPriceEpisodeStartDate < estimatedStartDate;

            await SetPriceEpisodeStartDate(priceEpisodeIdentifier, commitmentIdentifier, 1, StartDateIsBefore);
        }

        [Given(@"the start date in the (.*) is on or after the start date for Commitment (.*)")]
        public async Task GivenTheStartDateInThePEIsOnOrAfterTheStartDateForCommitmentA(string priceEpisodeIdentifier, string commitmentIdentifier)
        {
            bool StartDateIsOnOrAfter(DateTime actualPriceEpisodeStartDate, DateTime estimatedStartDate) => actualPriceEpisodeStartDate >= estimatedStartDate;
            
            await SetPriceEpisodeStartDate(priceEpisodeIdentifier, commitmentIdentifier, -1, StartDateIsOnOrAfter);
        }

        private async Task SetPriceEpisodeStartDate(string priceEpisodeIdentifier, string commitmentIdentifier, int numberOfDays, Func<DateTime, DateTime, bool> startDateIsAlreadyCorrect)
        {
            var actualPriceEpisodeStartDate = TestSession.FM36Global.Learners.Single().PriceEpisodes.Single(x => x.PriceEpisodeIdentifier == priceEpisodeIdentifier).PriceEpisodeValues.EpisodeEffectiveTNPStartDate.GetValueOrDefault();
            var apprenticeship = await testDataContext.Apprenticeship.Include(x => x.ApprenticeshipPriceEpisodes).SingleAsync(x => x.Id == TestSession.Apprenticeships[commitmentIdentifier].Id);

            if (startDateIsAlreadyCorrect(actualPriceEpisodeStartDate, apprenticeship.EstimatedStartDate)) return;

            apprenticeship.EstimatedStartDate = actualPriceEpisodeStartDate.AddDays(numberOfDays);
            apprenticeship.ApprenticeshipPriceEpisodes.Single().StartDate = actualPriceEpisodeStartDate.AddDays(numberOfDays);
            await testDataContext.SaveChangesAsync();
        }

        [Given("end date of (.*) and the start date of (.*) occur in the same month")]
        public void GivenEndDateOfEpisode1AndStartDateOfEpisode2OccurInTheSameMonth(string priceEpisodeIdentifier1, string priceEpisodeIdentifier2)
        {
            //NOTE: This is done else where in another step definition 
        }

        [Given("both (.*) and (.*) in the ILR matches to both Commitments (.*) and (.*), on ULN and UKPRN")]
        public async Task GivenTwoPriceEpisodeInIlrMatchesTwoCommitments(string priceEpisodeIdentifier1, string priceEpisodeIdentifier2, string commitmentIdentifier1, string commitmentIdentifier2)
        {
            await SetupTestData(priceEpisodeIdentifier1, priceEpisodeIdentifier2, commitmentIdentifier1, commitmentIdentifier2);
        }

        [Given("price episode (.*) in the ILR matches to both Commitments (.*) and (.*), on ULN and UKPRN")]
        public async Task GivenPriceEpisodeInIlrMatchesCommitments(string priceEpisodeIdentifier, string commitmentIdentifier1, string commitmentIdentifier2)
        {
            await SetupTestData(priceEpisodeIdentifier, null, commitmentIdentifier1, commitmentIdentifier2);
        }

        private async Task SetupTestData(string priceEpisodeIdentifier1, string priceEpisodeIdentifier2, string commitmentIdentifier1, string commitmentIdentifier2)
        {
            var learner = TestSession.FM36Global.Learners.Single();
            learner.ULN = TestSession.Learner.Uln;
            learner.LearnRefNumber = TestSession.Learner.LearnRefNumber;

            var priceEpisode1 = learner.PriceEpisodes.Single(y => y.PriceEpisodeIdentifier == priceEpisodeIdentifier1);
            var learningDelivery1 = learner.LearningDeliveries.Single(x => x.AimSeqNumber == priceEpisode1.PriceEpisodeValues.PriceEpisodeAimSeqNumber);

            LearningDelivery learningDelivery2;
            PriceEpisode priceEpisode2;

            if (string.IsNullOrWhiteSpace(priceEpisodeIdentifier2))
            {
                priceEpisode2 = priceEpisode1;
                learningDelivery2 = learningDelivery1;
            }
            else
            {
                priceEpisode2 = learner.PriceEpisodes.Single(y => y.PriceEpisodeIdentifier == priceEpisodeIdentifier2);
                learningDelivery2 = learner.LearningDeliveries.Single(x => x.AimSeqNumber == priceEpisode2.PriceEpisodeValues.PriceEpisodeAimSeqNumber);
            }

            var ids = new List<long> { TestSession.GenerateId(), TestSession.GenerateId() };

            var commitment1 = new ApprenticeshipBuilder()
                              .BuildSimpleApprenticeship(TestSession, learningDelivery1.LearningDeliveryValues, ids.Min())
                              .WithALevyPayingEmployer()
                              .WithApprenticeshipPriceEpisode(priceEpisode1.PriceEpisodeValues)
                              .ToApprenticeshipModel();

            var commitment2 = new ApprenticeshipBuilder()
                              .BuildSimpleApprenticeship(TestSession, learningDelivery2.LearningDeliveryValues, ids.Max())
                              .WithALevyPayingEmployer()
                              .WithApprenticeshipPriceEpisode(priceEpisode2.PriceEpisodeValues)
                              .ToApprenticeshipModel();

            TestSession.Apprenticeships.GetOrAdd(commitmentIdentifier1, commitment1);
            testDataContext.Apprenticeship.Add(commitment1);
            testDataContext.ApprenticeshipPriceEpisode.AddRange(commitment1.ApprenticeshipPriceEpisodes);

            TestSession.Apprenticeships.GetOrAdd(commitmentIdentifier2, commitment2);
            testDataContext.Apprenticeship.Add(commitment2);
            testDataContext.ApprenticeshipPriceEpisode.AddRange(commitment2.ApprenticeshipPriceEpisodes);

            var levyModel = TestSession.Employer.ToModel();
            levyModel.Balance = 1000000000;
            testDataContext.LevyAccount.Add(levyModel);
            await testDataContext.SaveChangesAsync();

            TestSession.FM36Global.UKPRN = TestSession.Provider.Ukprn;
        }

        [When(@"the Provider submits the single price episode PE-1 in the ILR")]
        [When("the Provider submits the 2 price episodes in the ILR")]
        public async Task WhenTheProviderSubmitsThePriceEpisodesInTheIlr()
        {
            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                                             TestSession.Provider.Ukprn,
                                             TestSession.CollectionPeriod.AcademicYear,
                                             TestSession.CollectionPeriod.Period,
                                             TestSession.Provider.JobId);
        }

        private bool HasDataLockErrorsForPriceEpisode(string priceEpisodeIdentifier, short academicYear)
        {
            var result = EarningEventsHelper
                         .GetOnProgrammeDataLockErrorsForLearnerAndPriceEpisode(priceEpisodeIdentifier, academicYear, TestSession)
                         .Any();
            return result;
        }

        private bool HasSingleMatchForPriceEpisodeAndCommitment(string priceEpisodeIdentifier)
        {
            var result = EarningEventsHelper
                         .PayableEarningsReceivedForLearner(TestSession)
                         .Count(x => x.PriceEpisodes.Any(y => y.Identifier == priceEpisodeIdentifier)) == 1;
            return result;
        }

        [Then("there is a single match for (.*) with Commitment (.*)")]
        public async Task ThereIsASingleMatchForPeWithCommitment(string priceEpisodeIdentifier, string commitmentIdentifier)
        {
            await WaitForIt(() =>
                                !HasDataLockErrorsForPriceEpisode(priceEpisodeIdentifier, short.Parse(TestSession.FM36Global.Year))
                             && HasSingleMatchForPriceEpisodeAndCommitment(priceEpisodeIdentifier),
                            "Failed to find a matching earning event and no datalocks.");
        }

        private bool HasDLock9ErrorForPriceEpisode(string priceEpisodeIdentifier, short academicYear)
        {
            var result = EarningEventsHelper
                   .GetOnProgrammeDataLockErrorsForLearnerAndPriceEpisode(priceEpisodeIdentifier, academicYear, TestSession)
                   .Any(x => x == DataLockErrorCode.DLOCK_09);
            return result;
        }

        private bool HasNoEarningEventMatch(string priceEpisodeIdentifier)
        {
            var result = !EarningEventsHelper.PayableEarningsReceivedForLearner(TestSession)
                                       .Any(x => x.PriceEpisodes.Any(y => y.Identifier == priceEpisodeIdentifier)
                                              && x.OnProgrammeEarnings.Any(d => d.Periods.Any(p => p.Amount != 0))
                                              && x.IncentiveEarnings.Any(d => d.Periods.Any(p => p.Amount != 0)));
            return result;
        }

        [Then("there is a DLOCK-09 triggered for (.*) and no match in DAS")]
        public async Task ThenThereIsNoMatchForPriceEpisodeInDas(string priceEpisodeIdentifier)
        {
            await WaitForIt(() => HasDLock9ErrorForPriceEpisode(priceEpisodeIdentifier, short.Parse(TestSession.FM36Global.Year))
                               && HasNoEarningEventMatch(priceEpisodeIdentifier),
                            "Failed to find a matching DLOCK-09 event and no earning events.");
        }

        public void Dispose()
        {
            ((IDisposable)featureContext)?.Dispose();
            testDataContext?.Dispose();
        }
    }
}