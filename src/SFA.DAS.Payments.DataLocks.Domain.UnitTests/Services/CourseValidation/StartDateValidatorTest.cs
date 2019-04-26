using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public void ReturnsMatchedPriceEpisodes()
        {
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { StartDate = startDate, Identifier = priceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 99,
                            StartDate = startDate.AddDays(-1)
                        }
                    }
                }
            };

            var validator = new StartDateValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull();
            result.ApprenticeshipPriceEpisodes.Any(ape => ape.Id == 99).Should().BeTrue();
        }

        [Test]
        public void AssignsCoveringPriceEpisodes()
        {
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { StartDate = startDate, Identifier = priceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
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
                            Id = 99,
                            StartDate = startDate.AddDays(-2),
                            EndDate = startDate.AddDays(-1)
                        },
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 100,
                            StartDate = startDate
                        },
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 98,
                            StartDate = startDate.AddDays(-4),
                            EndDate = startDate.AddDays(-3),
                        },
                    }
                }
            };

            var validator = new StartDateValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull();
            result.ApprenticeshipPriceEpisodes.Any(ape => ape.Id == 99).Should().BeTrue();
            result.ApprenticeshipPriceEpisodes.Any(ape => ape.Id == 100).Should().BeTrue();
        }

        [Test]
        public void ReturnsDataLockErrorWhenNoCoveringPriceEpisodeFound()
        {

            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { StartDate = startDate, Identifier = priceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            StartDate = startDate.AddDays(1)
                        }
                    }
                }
            };

            var validator = new StartDateValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().NotBeNull();
            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_09);
        }

        [Test]
        public void ShouldNotMatchRevmovedApprenticeshipPriceEpisodes()
        {
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { StartDate = startDate, Identifier = priceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            StartDate = startDate.AddDays(-1),
                            Removed = true
                        }
                    }
                }
            };

            var validator = new StartDateValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().NotBeNull();
            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_09);
        }
    }
}