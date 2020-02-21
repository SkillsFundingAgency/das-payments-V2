using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using Moq;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class StartDateValidatorTest
    {
        private readonly string priceEpisodeIdentifier = "pe-1";
        private EarningPeriod period;
        private Mock<ICalculatePeriodStartAndEndDate> calculatePeriodStartAndEndDate;

        [SetUp]
        public void Setup()
        {
            period = new EarningPeriod
            {
                Period = 1
            };

            calculatePeriodStartAndEndDate = new Mock<ICalculatePeriodStartAndEndDate>();
            calculatePeriodStartAndEndDate
                .Setup(x => x.GetPeriodDate(1, 1819))
                .Returns(() => (new DateTime(2018, 8, 1), new DateTime(2018, 8, 31)));
        }

        [AutoData]
        public void ReturnsOnlyCommitmentsThatStartOnOrBeforePriceEpisode(List<ApprenticeshipModel> apprenticeshipModels, PriceEpisode priceEpisode)
        {
            priceEpisode.EffectiveTotalNegotiatedPriceStartDate = new DateTime(2018, 8, 1);

            apprenticeshipModels[0].ApprenticeshipPriceEpisodes[0].StartDate = new DateTime(2018, 6, 1);
            apprenticeshipModels[0].ApprenticeshipPriceEpisodes[0].EndDate = new DateTime(2018, 8, 1);

            apprenticeshipModels[1].ApprenticeshipPriceEpisodes[0].StartDate = new DateTime(2018, 8, 1);
            apprenticeshipModels[1].ApprenticeshipPriceEpisodes[0].EndDate = new DateTime(2019, 8, 30);

            apprenticeshipModels[2].ApprenticeshipPriceEpisodes[0].StartDate = new DateTime(2019, 9, 1);
            apprenticeshipModels[2].ApprenticeshipPriceEpisodes[0].EndDate = new DateTime(2020, 9, 30);

            var validator = new StartDateValidator();
            (List<ApprenticeshipModel> validApprenticeships, List<DataLockFailure> dataLockFailures) result = validator.Validate(priceEpisode, apprenticeshipModels);

            result.dataLockFailures.Should().BeEmpty();
            result.validApprenticeships.Should().NotBeNull();
            result.validApprenticeships.Any(x => x.Id == apprenticeshipModels[0].Id).Should().BeTrue();
            result.validApprenticeships.Any(x => x.Id == apprenticeshipModels[1].Id).Should().BeTrue();
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

            var validator = new StartDateValidator();
            (List<ApprenticeshipModel> validApprenticeships, List<DataLockFailure> dataLockFailures) result = validator
                .Validate(priceEpisode, apprenticeshipModels);

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
                    ApprenticeshipId = apprenticeshipModels[1].Id,
                    ApprenticeshipPriceEpisodeIds = new List<long>() { apprenticeshipB.ApprenticeshipPriceEpisodes[0].Id },
                    DataLockError = DataLockErrorCode.DLOCK_09,
                });
        }


        //[Test]
        //public void ApprenticeshipsNotWithinADeliveryPeriodShouldReturnDLock09()
        //{
        //    var priceEpisode = new PriceEpisode
        //    {
        //        EffectiveTotalNegotiatedPriceStartDate = DateTime.Today,
        //        Identifier = priceEpisodeIdentifier
        //    };

        //    var apprenticeship = new ApprenticeshipModel
        //    {
        //        Id = 1,
        //        AccountId = 21,
        //        ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
        //        {
        //            new ApprenticeshipPriceEpisodeModel {Id = 90},
        //            new ApprenticeshipPriceEpisodeModel {Id = 91},
        //            new ApprenticeshipPriceEpisodeModel {Id = 92},
        //            new ApprenticeshipPriceEpisodeModel {Id = 93},
        //        },
        //        EstimatedStartDate = new DateTime(2018, 9, 15)
        //    };
           
        //    var validator = new StartDateValidator();
        //    var result = validator.Validate(priceEpisode,new List<ApprenticeshipModel>{apprenticeship});
        //    result.dataLockFailures.Should().NotBeNull();
        //    result.dataLockFailures[0].DataLockError.Should().Be(DataLockErrorCode.DLOCK_09);
        //    result.validApprenticeships.Should().BeEmpty();
        //}


    }
}