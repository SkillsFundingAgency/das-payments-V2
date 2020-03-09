using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Tests.Core;
using SFA.DAS.Payments.Tests.Core.Builders;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    // ReSharper disable once InconsistentNaming
    public class PV2_1826_Steps : FM36_ILR_Base_Steps
    {
     
        public PV2_1826_Steps(FeatureContext context) : base(context)
        {
        }

        [Given(@"there are 2 price episodes in the ILR submitted for (.*), PE-1 and PE-2")]
        public void GivenAreTwoPriceEpisodesInTheILR(string collectionPeriodText)
        {
            GetFM36LearnerForCollectionPeriod(collectionPeriodText);
        }

        [Given("price episode (.*) in the ILR matches to both Commitments (.*) and (.*), on ULN and UKPRN")]
        public async Task GivenPriceEpisodeInIlrMatchesCommitments(string priceEpisodeIdentifier, string commitmentIdentifier1, string commitmentIdentifier2)
        {
            var learner = TestSession.FM36Global.Learners.Single();
            learner.ULN = TestSession.Learner.Uln;
            learner.LearnRefNumber = TestSession.Learner.LearnRefNumber;

            var priceEpisode = learner.PriceEpisodes.Single(y => y.PriceEpisodeIdentifier == priceEpisodeIdentifier);
            var learningDelivery = learner.LearningDeliveries.Single(x => x.AimSeqNumber == priceEpisode.PriceEpisodeValues.PriceEpisodeAimSeqNumber);

            var ids = new List<long>{ TestSession.GenerateId(), TestSession.GenerateId() };
            
            var commitment1 = new ApprenticeshipBuilder().BuildSimpleApprenticeship(TestSession, learningDelivery.LearningDeliveryValues, ids.Min()).WithALevyPayingEmployer().WithApprenticeshipPriceEpisode(priceEpisode.PriceEpisodeValues).ToApprenticeshipModel();
            var commitment2 = new ApprenticeshipBuilder().BuildSimpleApprenticeship(TestSession, learningDelivery.LearningDeliveryValues, ids.Max()).WithALevyPayingEmployer().WithApprenticeshipPriceEpisode(priceEpisode.PriceEpisodeValues).ToApprenticeshipModel();

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

        private bool HasDLock9ErrorForPriceEpisode(string priceEpisodeIdentifier, short academicYear)
        {
            return EarningEventsHelper
                   .GetOnProgrammeDataLockErrorsForLearnerAndPriceEpisode(priceEpisodeIdentifier, academicYear, TestSession)
                   .Any(x => x == DataLockErrorCode.DLOCK_09);
        }

        private bool HasNoEarningEventMatch(string priceEpisodeIdentifier)
        {
            return !EarningEventsHelper.PayableEarningsReceivedForLearner(TestSession)
                .Any(x => x.PriceEpisodes.Any(y => y.Identifier == priceEpisodeIdentifier) &&
                          x.OnProgrammeEarnings.Any(d => d.Periods.Any(p => p.Amount != 0)) &&
                          x.IncentiveEarnings.Any(d => d.Periods.Any(p => p.Amount != 0))
                );
        }

        [Then("there is a DLOCK-09 triggered for (.*) and no match in DAS")]
        public async Task ThenThereIsNoMatchForPriceEpisodeInDas(string priceEpisodeIdentifier)
        {
            await WaitForIt(() => HasDLock9ErrorForPriceEpisode(priceEpisodeIdentifier, short.Parse(TestSession.FM36Global.Year))
                               && HasNoEarningEventMatch(priceEpisodeIdentifier),
                            "Failed to find a matching DLOCK-09 event and no earning events.");
        }

       
    }
}