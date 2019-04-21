using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class StartDateValidatorTest
    {
        private readonly DateTime startDate = DateTime.Today;
        private readonly string priceEpisodeIdentifier = "pe-1";
        private EarningPeriod period;

        [SetUp]
        public void Setup()
        {
            period = new EarningPeriod
            {
                Period = 1
            };
        }

        [Test]
        public void ReturnsNoDataLockErrors()
        {
            var apprenticeship = new ApprenticeshipModel
            {
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        Id = 99,
                        StartDate = startDate.AddDays(-1)
                    }
                }
            };

            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { StartDate = startDate, Identifier = priceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = apprenticeship
            };

            var validator = new StartDateValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull();
            result.ApprenticeshipPriceEpisodeIdentifier.Should().Be(99);
        }

        [Test]
        public void AssignsMostRecentCoveringPriceEpisode()
        {
            var apprenticeship = new ApprenticeshipModel
            {
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        Id = 97,
                        StartDate = startDate.AddDays(-6),
                        EndDate = startDate.AddDays(-5),
                    },
                    new ApprenticeshipPriceEpisodeModel
                    {
                        Id = 98,
                        StartDate = startDate.AddDays(-4),
                        EndDate = startDate.AddDays(-3),
                    },
                    new ApprenticeshipPriceEpisodeModel
                    {
                        Id = 99,
                        StartDate = startDate.AddDays(-2)
                    },
                }
            };

            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { StartDate = startDate, Identifier = priceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = apprenticeship
            };

            var validator = new StartDateValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull();
            result.ApprenticeshipPriceEpisodeIdentifier.Should().Be(99);
        }

        [Test]
        public void ReturnsDataLockErrorWhenNoCoveringPriceEpisodeFound()
        {
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

            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { StartDate = startDate, Identifier = priceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = apprenticeship
            };

            var validator = new StartDateValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().NotBeNull();
            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_09);
        }

    }
}