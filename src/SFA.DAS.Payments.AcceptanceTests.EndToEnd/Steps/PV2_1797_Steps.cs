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