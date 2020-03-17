using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers;
using SFA.DAS.Payments.Tests.Core;
using SFA.DAS.Payments.Tests.Core.Builders;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    // ReSharper disable once InconsistentNaming
    public class PV2_1797_Steps : FM36_ILR_Base_Steps
    {
        public PV2_1797_Steps(FeatureContext context) : base(context) { }

        [Given(@"there are 2 Commitments in DAS, Commitment A and Commitment B in collection period (.*)")]
        public void GivenThereAreCommitmentsInDASCommitmentAAndCommitmentBInCollectionPeriodRCurrentAcademicYear( string collectionPeriodText)
        {
            GetFM36LearnerForCollectionPeriod(collectionPeriodText);
        }
        
        [Given(@"there is a single price episode in the ILR, (.*)")]
        public void GivenThereIsASinglePriceEpisodeInTheILRPE(string p0) { }


        [Given(@"the (.*) in the ILR matches to both Commitments (.*) and (.*), on ULN and UKPRN")]
        public async Task GivenThePEInTheILRMatchesToBothCommitmentsAAndBOnULNAndUKPRN(string priceEpisodeIdentifier1, string commitmentIdentifier1, string commitmentIdentifier2)
        {
            var learner = TestSession.FM36Global.Learners.Single();
            learner.ULN = TestSession.Learner.Uln;
            learner.LearnRefNumber = TestSession.Learner.LearnRefNumber;

            var priceEpisode1 = learner.PriceEpisodes.Single(y => y.PriceEpisodeIdentifier == priceEpisodeIdentifier1);
            var learningDelivery1 = learner.LearningDeliveries.Single(x => x.AimSeqNumber == priceEpisode1.PriceEpisodeValues.PriceEpisodeAimSeqNumber);

            var ids = new List<long> { TestSession.GenerateId(), TestSession.GenerateId() };
            var commitment1 = new ApprenticeshipBuilder().BuildSimpleApprenticeship(TestSession, learningDelivery1.LearningDeliveryValues, ids.Min()).WithALevyPayingEmployer().WithApprenticeshipPriceEpisode(priceEpisode1.PriceEpisodeValues).ToApprenticeshipModel();
            var commitment2 = new ApprenticeshipBuilder().BuildSimpleApprenticeship(TestSession, learningDelivery1.LearningDeliveryValues, ids.Max()).WithALevyPayingEmployer().WithApprenticeshipPriceEpisode(priceEpisode1.PriceEpisodeValues).ToApprenticeshipModel();

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

        [Given(@"the start date in the (.*) is on or after the start date for Commitment (.*)")]
        public async Task GivenTheStartDateInThePEIsOnOrAfterTheStartDateForCommitmentA(string priceEpisodeIdentifier, string commitmentIdentifier)
        {
            var actualPriceEpisodeStartDate = TestSession.FM36Global.Learners.Single().PriceEpisodes.Single(x => x.PriceEpisodeIdentifier == priceEpisodeIdentifier).PriceEpisodeValues.EpisodeEffectiveTNPStartDate;
            var apprenticeship = await testDataContext.Apprenticeship.Include(x => x.ApprenticeshipPriceEpisodes).SingleAsync(x => x.Id == TestSession.Apprenticeships[commitmentIdentifier].Id);

            if (actualPriceEpisodeStartDate.GetValueOrDefault() >= apprenticeship.EstimatedStartDate) return;

            apprenticeship.EstimatedStartDate = actualPriceEpisodeStartDate.GetValueOrDefault().AddDays(-1);
            apprenticeship.ApprenticeshipPriceEpisodes.Single().StartDate = actualPriceEpisodeStartDate.GetValueOrDefault().AddDays(-1);

            await testDataContext.SaveChangesAsync();
        }

        [Given(@"the start date in the (.*) is before the start date for Commitment (.*)")]
        public async Task GivenTheStartDateInThePEIsBeforeTheStartDateForCommitmentB(string priceEpisodeIdentifier, string commitmentIdentifier)
        {
            var actualPriceEpisodeStartDate = TestSession.FM36Global.Learners.Single().PriceEpisodes.Single(x => x.PriceEpisodeIdentifier == priceEpisodeIdentifier).PriceEpisodeValues.EpisodeEffectiveTNPStartDate;
            var apprenticeship = await testDataContext.Apprenticeship.Include(x => x.ApprenticeshipPriceEpisodes).SingleAsync(x => x.Id == TestSession.Apprenticeships[commitmentIdentifier].Id);
            
            if(actualPriceEpisodeStartDate.GetValueOrDefault() < apprenticeship.EstimatedStartDate) return;
            
            apprenticeship.EstimatedStartDate = actualPriceEpisodeStartDate.GetValueOrDefault().AddDays(1);
            apprenticeship.ApprenticeshipPriceEpisodes.Single().StartDate = actualPriceEpisodeStartDate.GetValueOrDefault().AddDays(1);
            await testDataContext.SaveChangesAsync();
        }
        
    }

}