using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class StartDateValidatorTest
    {
      
        [AutoData]
        public void ValidCommitmentsShouldBeCommitmentsThatStartOnOrBeforePriceEpisode(
            List<ApprenticeshipModel> apprenticeshipModels, PriceEpisode priceEpisode)
        {
            priceEpisode.EffectiveTotalNegotiatedPriceStartDate = new DateTime(2018, 8, 1);

            apprenticeshipModels[0].ApprenticeshipPriceEpisodes[0].StartDate = new DateTime(2018, 6, 1);
            apprenticeshipModels[0].ApprenticeshipPriceEpisodes[0].EndDate = new DateTime(2018, 8, 1);
            apprenticeshipModels[0].ApprenticeshipPriceEpisodes[0].Removed = false;
            apprenticeshipModels[0].ApprenticeshipPriceEpisodes.RemoveRange(1, 2);

            apprenticeshipModels[1].ApprenticeshipPriceEpisodes[0].StartDate = new DateTime(2018, 8, 1);
            apprenticeshipModels[1].ApprenticeshipPriceEpisodes[0].EndDate = new DateTime(2019, 8, 30);
            apprenticeshipModels[1].ApprenticeshipPriceEpisodes[0].Removed = false;
            apprenticeshipModels[1].ApprenticeshipPriceEpisodes.RemoveRange(1, 2);

            apprenticeshipModels[2].ApprenticeshipPriceEpisodes[0].StartDate = new DateTime(2019, 9, 1);
            apprenticeshipModels[2].ApprenticeshipPriceEpisodes[0].EndDate = new DateTime(2020, 9, 30);
            apprenticeshipModels[2].ApprenticeshipPriceEpisodes[0].Removed = false;
            apprenticeshipModels[2].ApprenticeshipPriceEpisodes.RemoveRange(1, 2);

            var validator = new StartDateValidator(false);
            var result = validator.Validate(priceEpisode, apprenticeshipModels);

            result.dataLockFailures.Should().BeEmpty();
            result.validApprenticeships.Should().HaveCount(2);
            result.validApprenticeships.Any(x => x.Id == apprenticeshipModels[0].Id).Should().BeTrue();
            result.validApprenticeships.Any(x => x.Id == apprenticeshipModels[1].Id).Should().BeTrue();
        }

        [AutoData]
        public void ReturnsAllCommitmentsWhenDisableDatalocksAreOn(ApprenticeshipModel apprenticeshipA, ApprenticeshipModel apprenticeshipB, PriceEpisode priceEpisode)
        {
            priceEpisode.EffectiveTotalNegotiatedPriceStartDate = new DateTime(1930, 8, 1);
            var apprenticeshipModels = new List<ApprenticeshipModel> { apprenticeshipA, apprenticeshipB };

            var validator = new StartDateValidator(true);
            var result = validator.Validate(priceEpisode, apprenticeshipModels);

            result.validApprenticeships.Should().BeEquivalentTo(apprenticeshipModels);
        }

        [AutoData]
        public void ReturnsDLock09WhenNoCommitmentsStartOnOrBeforePriceEpisode(ApprenticeshipModel apprenticeshipA, ApprenticeshipModel apprenticeshipB,PriceEpisode priceEpisode)
        {
            priceEpisode.EffectiveTotalNegotiatedPriceStartDate = new DateTime(2018, 8, 1);

            apprenticeshipA.ApprenticeshipPriceEpisodes.ForEach(x => x.Removed =true);
            apprenticeshipA.ApprenticeshipPriceEpisodes[0].StartDate = new DateTime(2018, 9, 1);
            apprenticeshipA.ApprenticeshipPriceEpisodes[0].EndDate = new DateTime(2019, 9, 30);
            apprenticeshipA.ApprenticeshipPriceEpisodes[0].Removed = false;
            
            apprenticeshipB.ApprenticeshipPriceEpisodes.ForEach(x => x.Removed = true);
            apprenticeshipB.ApprenticeshipPriceEpisodes[0].StartDate = new DateTime(2019, 9, 1);
            apprenticeshipB.ApprenticeshipPriceEpisodes[0].EndDate = new DateTime(2020, 9, 30);
            apprenticeshipB.ApprenticeshipPriceEpisodes[0].Removed = false;
            
            List<ApprenticeshipModel> apprenticeshipModels = new List<ApprenticeshipModel> {apprenticeshipA, apprenticeshipB};

            var validator = new StartDateValidator(false);
            var  result = validator.Validate(priceEpisode, apprenticeshipModels);

            result.validApprenticeships.Should().BeEmpty();
            result.dataLockFailures.Should().HaveCount(2);
            result.dataLockFailures.Should().ContainEquivalentOf(
                new
                {
                    ApprenticeshipId = apprenticeshipA.Id,
                    ApprenticeshipPriceEpisodeIds = new List<long>(){ apprenticeshipA.ApprenticeshipPriceEpisodes[0].Id},
                    DataLockError = DataLockErrorCode.DLOCK_09,
                });

            result.dataLockFailures.Should().ContainEquivalentOf(
                new
                {
                    ApprenticeshipId = apprenticeshipB.Id,
                    ApprenticeshipPriceEpisodeIds = new List<long>() { apprenticeshipB.ApprenticeshipPriceEpisodes[0].Id },
                    DataLockError = DataLockErrorCode.DLOCK_09,
                });
        }
    }
}