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
                EstimatedStartDate = startDate.AddDays(-1),
                EstimatedEndDate = startDate.AddDays(1)
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

            result.Count.Should().Equals(0);
        }

        [Test]
        public void ReturnsDataLockErrorWhenExpired()
        {
            var startDate = DateTime.UtcNow;
            var priceEpisodeIdentifier = "pe-1";
            byte period = 1;

            var apprenticeship = new ApprenticeshipModel
            {
                EstimatedStartDate = startDate.AddDays(1),
                EstimatedEndDate = startDate.AddDays(2),
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
            result.First().DataLockErrorCode.Should().Be(true);
            result.First().DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_09);
        }

        [Test]
        public void ReturnsDataLockErrorWheNotBegun()
        {
            var startDate = DateTime.UtcNow;
            var priceEpisodeIdentifier = "pe-1";
            byte period = 1;

            var apprenticeship = new ApprenticeshipModel
            {
                EstimatedStartDate = startDate.AddDays(-10),
                EstimatedEndDate = startDate.AddDays(-1)
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
            result.First().DataLockErrorCode.Should().Be(true);
            result.First().DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_09);
        }

        [Test]
        public void ReturnsDataLockErrorsWhenGapBetweenApprenticeships()
        {
            var startDate = DateTime.UtcNow;
            var priceEpisodeIdentifier = "pe-1";
            byte period = 1;

            var earlyApprenticeship = new ApprenticeshipModel
            {
                EstimatedStartDate = startDate.AddDays(-10),
                EstimatedEndDate = startDate.AddDays(-1)
            };

            var lateApprenticeship = new ApprenticeshipModel
            {
                EstimatedStartDate = startDate.AddDays(2),
                EstimatedEndDate = startDate.AddDays(10)
            };

            var apprenticeships = new List<ApprenticeshipModel> { earlyApprenticeship, lateApprenticeship };

            var validation = new CourseValidation
            {
                PriceEpisode = new PriceEpisode { StartDate = startDate, Identifier = priceEpisodeIdentifier },
                Period = period,
                Apprenticeships = apprenticeships
            };

            var validator = new StartDateValidator();

            var result = validator.Validate(validation);

            result.Count.Should().Equals(2);

            var firstError = result.First();

            firstError.DataLockErrorCode.Should().Be(true);
            firstError.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_09);

            var secondError = result.Last();

            secondError.DataLockErrorCode.Should().Be(true);
            secondError.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_09);
        }

        [Test]
        public void ReturnsDataLockErrorWhenGapBetweenApprenticeshipsButStartDateMatchesOne()
        {
            var startDate = DateTime.UtcNow.AddDays(-5);
            var priceEpisodeIdentifier = "pe-1";
            byte period = 1;

            var earlyApprenticeship = new ApprenticeshipModel
            {
                EstimatedStartDate = startDate.AddDays(-10),
                EstimatedEndDate = startDate.AddDays(-1)
            };

            var lateApprenticeship = new ApprenticeshipModel
            {
                EstimatedStartDate = startDate.AddDays(2),
                EstimatedEndDate = startDate.AddDays(10)
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

            firstError.DataLockErrorCode.Should().Be(true);
            firstError.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_09);
        }
    }
}
