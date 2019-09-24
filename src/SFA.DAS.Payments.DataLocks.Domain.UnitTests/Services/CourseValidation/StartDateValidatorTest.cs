using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class StartDateValidatorTest
    {
        private readonly DateTime startDate = DateTime.Today;
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

        [Test]
        public void ReturnsMatchedPriceEpisodes()
        {
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { EffectiveTotalNegotiatedPriceStartDate = startDate, Identifier = priceEpisodeIdentifier },
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

            var validator = new StartDateValidator(calculatePeriodStartAndEndDate.Object);
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull();
            result.ApprenticeshipPriceEpisodes.Any(ape => ape.Id == 99).Should().BeTrue();
        }

        [Test]
        public void AssignsAllValidPriceEpisodes()
        {
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { EffectiveTotalNegotiatedPriceStartDate = startDate, Identifier = priceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 1,
                            StartDate = startDate.AddDays(-6),
                            EndDate = startDate.AddDays(-5),
                        },
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 2,
                            StartDate = startDate.AddDays(-2),
                            EndDate = startDate.AddDays(-1)
                        },
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 3,
                            StartDate = startDate
                        },
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 4,
                            StartDate = startDate.AddDays(-4),
                            EndDate = startDate.AddDays(-3),
                        },
                    }
                }
            };

            var validator = new StartDateValidator(calculatePeriodStartAndEndDate.Object);
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull();
            result.ApprenticeshipPriceEpisodes[0].Id.Should().Be(1);
            result.ApprenticeshipPriceEpisodes[1].Id.Should().Be(2);
            result.ApprenticeshipPriceEpisodes[2].Id.Should().Be(3);
            result.ApprenticeshipPriceEpisodes[3].Id.Should().Be(4);
        }

        [Test]
        public void ReturnsDataLockErrorWhenNoCoveringPriceEpisodeFound()
        {

            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { EffectiveTotalNegotiatedPriceStartDate = startDate, Identifier = priceEpisodeIdentifier },
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

            var validator = new StartDateValidator(calculatePeriodStartAndEndDate.Object);
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().NotBeNull();
            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_09);
        }

        [Test]
        public void ShouldNotMatchRemovedApprenticeshipPriceEpisodes()
        {
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { EffectiveTotalNegotiatedPriceStartDate = startDate, Identifier = priceEpisodeIdentifier },
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

            var validator = new StartDateValidator(calculatePeriodStartAndEndDate.Object);
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().NotBeNull();
            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_09);
        }

        [Test]
        public void ApprenticeshipsNotWithinADeliveryPeriodShouldReturnDLock09()
        {
            period = new EarningPeriod
            {
                Period = 1
            };

            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode
                {
                    EffectiveTotalNegotiatedPriceStartDate = startDate,
                    Identifier = priceEpisodeIdentifier
                },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
                    AccountId = 21,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 90},
                        new ApprenticeshipPriceEpisodeModel{Id = 91},
                        new ApprenticeshipPriceEpisodeModel{Id = 92},
                        new ApprenticeshipPriceEpisodeModel{Id = 93},
                    },
                    EstimatedStartDate = new DateTime(2018, 9, 15)
                }
            };
            
            calculatePeriodStartAndEndDate
              .Setup(x => x.GetPeriodDate(1, 1819))
              .Returns(() => (new DateTime(2018, 8, 1), new DateTime(2018, 8, 31)));
            

            var validator = new StartDateValidator(calculatePeriodStartAndEndDate.Object);
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().NotBeNull();
            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_09);
            result.ApprenticeshipPriceEpisodes.Should().BeEmpty();
        }
        
    }
}