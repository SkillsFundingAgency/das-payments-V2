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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Learner = SFA.DAS.Payments.AcceptanceTests.Core.Data.Learner;
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

            if (TestSession.FM36Global != null)
            {
                TestSession.PreviousFm36Global = TestSession.FM36Global;
            }

            TestSession.FM36Global = FM36GlobalDeserialiser.DeserialiseByFeatureForPeriod(featureContext.FeatureInfo.Title, TestSession.CollectionPeriod.Period.ToPeriodText());
            UpdateFm36ToUseLatestDates(TestSession.FM36Global);
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
            await SetupTestCommitmentData(commitmentIdentifier1, priceEpisodeIdentifier1, commitmentIdentifier2, priceEpisodeIdentifier2);
        }

        [Given("price episode (.*) in the ILR matches to both Commitments (.*) and (.*), on ULN and UKPRN")]
        public async Task GivenPriceEpisodeInIlrMatchesCommitments(string priceEpisodeIdentifier, string commitmentIdentifier1, string commitmentIdentifier2)
        {
            await SetupTestCommitmentData(commitmentIdentifier1, priceEpisodeIdentifier, commitmentIdentifier2, null);
        }

        protected async Task SetupTestCommitmentData(string commitmentIdentifier1, string priceEpisodeIdentifier1, string commitmentIdentifier2 = null, string priceEpisodeIdentifier2 = null, long tempUln = 0, Learner newLearner = null)
        {
            var overrideLearnerUln = tempUln != 0 && newLearner != null;

            var fm36Learner = TestSession.FM36Global.Learners.Single(l => tempUln == 0 || l.ULN == tempUln);

            if (overrideLearnerUln)
            {
                fm36Learner.ULN = newLearner.Uln;
                fm36Learner.LearnRefNumber = newLearner.LearnRefNumber;
            }
            else
            {
                fm36Learner.ULN = TestSession.Learner.Uln;
                fm36Learner.LearnRefNumber = TestSession.Learner.LearnRefNumber;
            }

            var priceEpisode1 = fm36Learner.PriceEpisodes.Single(y => y.PriceEpisodeIdentifier == priceEpisodeIdentifier1);
            var learningDelivery1 = fm36Learner.LearningDeliveries.Single(x => x.AimSeqNumber == priceEpisode1.PriceEpisodeValues.PriceEpisodeAimSeqNumber);

            LearningDelivery learningDelivery2;
            PriceEpisode priceEpisode2;

            if (string.IsNullOrWhiteSpace(priceEpisodeIdentifier2))
            {
                priceEpisode2 = priceEpisode1;
                learningDelivery2 = learningDelivery1;
            }
            else
            {
                priceEpisode2 = fm36Learner.PriceEpisodes.Single(y => y.PriceEpisodeIdentifier == priceEpisodeIdentifier2);
                learningDelivery2 = fm36Learner.LearningDeliveries.Single(x => x.AimSeqNumber == priceEpisode2.PriceEpisodeValues.PriceEpisodeAimSeqNumber);
            }

            var ids = new List<long> { TestSession.GenerateId(), TestSession.GenerateId() };

            var commitment1 = new ApprenticeshipBuilder()
                              .BuildSimpleApprenticeship(TestSession, learningDelivery1.LearningDeliveryValues, ids.Min())
                              .WithALevyPayingEmployer()
                              .WithApprenticeshipPriceEpisode(priceEpisode1.PriceEpisodeValues)
                              .WithLearnerUln(overrideLearnerUln ? newLearner.Uln : TestSession.Learner.Uln)
                              .ToApprenticeshipModel();

            TestSession.Apprenticeships.GetOrAdd(commitmentIdentifier1, commitment1);
            testDataContext.Apprenticeship.Add(commitment1);
            testDataContext.ApprenticeshipPriceEpisode.AddRange(commitment1.ApprenticeshipPriceEpisodes);

            ApprenticeshipModel commitment2 = null;
            if (commitmentIdentifier2 != null)
            {
                commitment2 = new ApprenticeshipBuilder()
                    .BuildSimpleApprenticeship(TestSession, learningDelivery2.LearningDeliveryValues, ids.Max())
                    .WithALevyPayingEmployer()
                    .WithApprenticeshipPriceEpisode(priceEpisode2.PriceEpisodeValues)
                    .WithLearnerUln(overrideLearnerUln ? newLearner.Uln : TestSession.Learner.Uln)
                    .ToApprenticeshipModel();

                TestSession.Apprenticeships.GetOrAdd(commitmentIdentifier2, commitment2);
                testDataContext.Apprenticeship.Add(commitment2);
                testDataContext.ApprenticeshipPriceEpisode.AddRange(commitment2.ApprenticeshipPriceEpisodes);
            }

            var levyModel = TestSession.Employer.ToModel();
            levyModel.Balance = 1000000000;

            if (!testDataContext.LevyAccount.Any(x => x.AccountId == levyModel.AccountId))
            {
                testDataContext.LevyAccount.Add(levyModel);
            }
            await testDataContext.SaveChangesAsync();

            TestSession.Apprenticeships[commitmentIdentifier1].ApprenticeshipPriceEpisodes = commitment1.ApprenticeshipPriceEpisodes;

            if (commitmentIdentifier2 != null)
            {
                TestSession.Apprenticeships[commitmentIdentifier1].ApprenticeshipPriceEpisodes = commitment2.ApprenticeshipPriceEpisodes;
            }

            TestSession.FM36Global.UKPRN = TestSession.Provider.Ukprn;
        }

        [Given(@"the employer has no levy balance")]
        public async Task GivenTheEmployerHasNoLevyBalance()
        {
            var levyModel = TestSession.Employer.ToModel();
            levyModel.Balance = 0;

            var existingLevyAccount = testDataContext.LevyAccount.FirstOrDefault(x => x.AccountId == levyModel.AccountId);

            if (existingLevyAccount == null)
            {
                await testDataContext.LevyAccount.AddAsync(levyModel);
            }
            else
            {
                existingLevyAccount.Balance = 0;
            }
            await testDataContext.SaveChangesAsync();
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

        protected async Task WaitForPayments(int count)
        {
            await WaitForIt(() => Scope.Resolve<IPaymentsHelper>().GetPaymentsCount(TestSession.Provider.Ukprn, TestSession.CollectionPeriod) == count,
                "Failed to wait for expected number of payments");
        }

        protected async Task WaitForRequiredPayments(int count)
        {
            await WaitForIt(() => Scope.Resolve<IPaymentsHelper>().GetRequiredPaymentsCount(TestSession.Provider.Ukprn, TestSession.CollectionPeriod) == count,
                "Failed to wait for expected number of required payments");
        }

        protected async Task WaitForUnexpectedRequiredPayments()
        {
            await WaitForUnexpected(() => (Scope.Resolve<IPaymentsHelper>().GetRequiredPaymentsCount(TestSession.Provider.Ukprn, TestSession.CollectionPeriod) == 0,
                "No required payments were expected"),
                "found unexpected number of required payments");
        }

        public void Dispose()
        {
            ((IDisposable)featureContext)?.Dispose();
            testDataContext?.Dispose();
        }

        private void UpdateFm36ToUseLatestDates(FM36Global fm36)
        {
            var currentAcademicYear = GetCurrentAcademicYear();
            var fm36AcademicYear = Convert.ToInt32(fm36.Year);
            int offset = GetYearFromAcademicYear(currentAcademicYear) - GetYearFromAcademicYear(fm36AcademicYear);

            if (offset == 0) return;

            fm36.Year = currentAcademicYear.ToString();
            fm36.RulebaseVersion = currentAcademicYear + fm36.RulebaseVersion.Substring(4);
            fm36.Learners.ForEach(learner =>
            {
                learner.PriceEpisodes.ForEach(priceEpisode =>
                {
                    priceEpisode.PriceEpisodeValues.EpisodeStartDate = priceEpisode.PriceEpisodeValues.EpisodeStartDate?.AddYears(offset);
                    priceEpisode.PriceEpisodeValues.PriceEpisodeActualEndDateIncEPA = priceEpisode.PriceEpisodeValues.PriceEpisodeActualEndDateIncEPA?.AddYears(offset);
                    priceEpisode.PriceEpisodeValues.PriceEpisodePlannedEndDate = priceEpisode.PriceEpisodeValues.PriceEpisodePlannedEndDate?.AddYears(offset);
                    priceEpisode.PriceEpisodeValues.PriceEpisodeActualEndDate = priceEpisode.PriceEpisodeValues.PriceEpisodeActualEndDate?.AddYears(offset);
                    priceEpisode.PriceEpisodeValues.EpisodeEffectiveTNPStartDate = priceEpisode.PriceEpisodeValues.EpisodeEffectiveTNPStartDate?.AddYears(offset);
                    priceEpisode.PriceEpisodeValues.PriceEpisodeSecondAdditionalPaymentThresholdDate = priceEpisode.PriceEpisodeValues.PriceEpisodeSecondAdditionalPaymentThresholdDate?.AddYears(offset);
                });

                learner.LearningDeliveries.ForEach(learningDelivery =>
                {
                    learningDelivery.LearningDeliveryValues.AdjStartDate = learningDelivery.LearningDeliveryValues.AdjStartDate?.AddYears(offset);
                    learningDelivery.LearningDeliveryValues.AppAdjLearnStartDate = learningDelivery.LearningDeliveryValues.AppAdjLearnStartDate?.AddYears(offset);
                    learningDelivery.LearningDeliveryValues.AppAdjLearnStartDateMatchPathway = learningDelivery.LearningDeliveryValues.AppAdjLearnStartDateMatchPathway?.AddYears(offset);

                    learningDelivery.LearningDeliveryValues.ApplicCompDate = learningDelivery.LearningDeliveryValues.ApplicCompDate != DateTime.MinValue || learningDelivery.LearningDeliveryValues.ApplicCompDate != DateTime.MaxValue ?
                            learningDelivery.LearningDeliveryValues.ApplicCompDate:
                            learningDelivery.LearningDeliveryValues.ApplicCompDate?.AddYears(offset);


                    learningDelivery.LearningDeliveryValues.LearnStartDate = learningDelivery.LearningDeliveryValues.LearnStartDate.AddYears(offset);
                    learningDelivery.LearningDeliveryValues.LearnDelApplicEmpDate = learningDelivery.LearningDeliveryValues.LearnDelApplicEmpDate?.AddYears(offset);
                    learningDelivery.LearningDeliveryValues.SecondIncentiveThresholdDate = learningDelivery.LearningDeliveryValues.SecondIncentiveThresholdDate?.AddYears(offset);

                    learner.HistoricEarningOutputValues.ForEach(historicEarningOutputValues =>
                    {
                        historicEarningOutputValues.HistoricEffectiveTNPStartDateOutput = historicEarningOutputValues.HistoricEffectiveTNPStartDateOutput?.AddYears(offset);
                        historicEarningOutputValues.HistoricProgrammeStartDateIgnorePathwayOutput = historicEarningOutputValues.HistoricProgrammeStartDateIgnorePathwayOutput?.AddYears(offset);
                        historicEarningOutputValues.HistoricProgrammeStartDateMatchPathwayOutput = historicEarningOutputValues.HistoricProgrammeStartDateMatchPathwayOutput?.AddYears(offset);
                        historicEarningOutputValues.HistoricUptoEndDateOutput = historicEarningOutputValues.HistoricUptoEndDateOutput?.AddYears(offset);
                    });
                });
            });
        }

        private int GetCurrentAcademicYear()
        {
            var year = DateTime.Now.Month < 8 ? DateTime.Now.Year - 1 : DateTime.Now.Year;
            var academicYear = string.Concat(year - 2000, year - 1999);
            return Convert.ToInt32(academicYear);
        }

        private int GetYearFromAcademicYear(int academicYear)
        {
            var academicYearString = academicYear.ToString().Substring(0, 2);
            var academicYearInt = Convert.ToInt32(academicYearString) + 2000;

            return academicYearInt;
        }
    }
}