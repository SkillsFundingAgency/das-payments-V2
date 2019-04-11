using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
{
    [TestFixture]
    public class StartDateValidatorTests
    {
        [Test]
        public void ReturnsNoDataLockErrors()
        {
            var startDate = DateTime.UtcNow;
            var priceEpisodeIdentifier = "pe-1";
            byte period = 1;

            var apprenticeship = new ApprenticeshipModel
            {
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        StartDate = startDate.AddDays(-1)
                    }
                }
            };

            var apprenticeships = new List<ApprenticeshipModel> { apprenticeship };

            var validation = new CourseValidation
            {
                PriceEpisode = new PriceEpisode {StartDate = startDate, Identifier = priceEpisodeIdentifier},
                Period = period,
                Apprenticeships = apprenticeships
            };

            var validator = new StartDateValidator();

            var result = validator.Validate(validation);

            result.Any().Should().BeFalse();
        }

        [Test]
        public void ReturnsDataLockErrorWheNotBegun()
        {
            var startDate = DateTime.UtcNow;
            var priceEpisodeIdentifier = "pe-1";
            byte period = 1;

            var apprenticeship = new ApprenticeshipModel
            {
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        StartDate = startDate.AddDays(1)
                    }
                }
            };

            var apprenticeships = new List<ApprenticeshipModel> { apprenticeship };

            var validation = new CourseValidation
            {
                PriceEpisode = new PriceEpisode { StartDate = startDate, Identifier = priceEpisodeIdentifier },
                Period = period,
                Apprenticeships = apprenticeships
            };

            var validator = new StartDateValidator();

            var result = validator.Validate(validation);

            result.Count.Should().Equals(1);
            result.First().DataLockErrorCode.HasValue.Should().Be(true);
            result.First().DataLockErrorCode.Value.Should().Be(DataLockErrorCode.DLOCK_09);
        }

        [Test]
        public void ReturnsDataLockErrorWhenGapBetweenApprenticeshipsButStartDateMatchesOne()
        {
            var startDate = DateTime.UtcNow.AddDays(-5);
            var priceEpisodeIdentifier = "pe-1";
            byte period = 1;

            var earlyApprenticeship = new ApprenticeshipModel
            {
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        StartDate = startDate.AddDays(-10)
                    }
                }
            };

            var lateApprenticeship = new ApprenticeshipModel
            {
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        StartDate = startDate.AddDays(2)
                    }
                }
            };

            var apprenticeships = new List<ApprenticeshipModel> {earlyApprenticeship, lateApprenticeship};

            var validation = new CourseValidation
            {
                PriceEpisode = new PriceEpisode { StartDate = startDate, Identifier = priceEpisodeIdentifier },
                Period = period,
                Apprenticeships = apprenticeships
            };

            var validator = new StartDateValidator();

            var result = validator.Validate(validation);

            result.Count.Should().Equals(1);

            var firstError = result.First();

            firstError.DataLockErrorCode.HasValue.Should().Be(true);
            firstError.DataLockErrorCode.Value.Should().Be(DataLockErrorCode.DLOCK_09);
        }
    }
}