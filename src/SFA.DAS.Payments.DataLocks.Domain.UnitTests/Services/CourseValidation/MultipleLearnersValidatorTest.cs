using System;
using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class MultipleLearnersValidatorTest
    {
        private  Mock<IDataLockLearnerCache> dataLockLearnerCache;
        private  Mock<ICalculatePeriodStartAndEndDate> calculatePeriodStartAndEndDate;
        private const string PriceEpisodeIdentifier = "pe-1";
        private EarningPeriod period;
        private const short academicYear = 1819;
        private const  long ukprn = 100;

        [SetUp]
        public void Setup()
        {
            period = new EarningPeriod
            {
                Period = 1
            };

            dataLockLearnerCache = new Mock<IDataLockLearnerCache>(MockBehavior.Strict);
            calculatePeriodStartAndEndDate = new Mock<ICalculatePeriodStartAndEndDate>(MockBehavior.Strict);
        }

        [TearDown]
        public void CleanUp()
        {
            dataLockLearnerCache.Verify();
            calculatePeriodStartAndEndDate.Verify();
        }

        [Test]
        public void WhenNoDuplicateApprenticeshipsReturnNoDataLockErrors()
        {
            var earningPeriod = new EarningPeriod {Period = 1};

            calculatePeriodStartAndEndDate
                .Setup(x => x.GetPeriodDate(earningPeriod.Period, academicYear))
                .Returns((DateTime.Parse("01 Aug 2018"), DateTime.Parse("31 Aug 2018")))
                .Verifiable();

            dataLockLearnerCache
                .Setup(x => x.GetDuplicateApprenticeships())
                .ReturnsAsync(new List<ApprenticeshipModel>
                {
                    new ApprenticeshipModel
                    {
                        Id = 1,
                        Uln = 200,
                        Ukprn = ukprn
                    }
                });

            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { Identifier = PriceEpisodeIdentifier },
                EarningPeriod = earningPeriod,
                AcademicYear = academicYear,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1 ,
                    Ukprn = ukprn,
                    Uln = 200,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 100,
                            StartDate = DateTime.Parse("01 Aug 2018")
                        }
                    }
                }
            };

            var validator = new MultipleLearnersValidator(dataLockLearnerCache.Object, calculatePeriodStartAndEndDate.Object);
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull();
            result.ApprenticeshipId.Should().Be(1);
            result.ApprenticeshipPriceEpisodes.Should().HaveCount(1);
            result.ApprenticeshipPriceEpisodes[0].Id.Should().Be(100);
        }

        [TestCase("When Apprenticeship start date is in a prior earning period then no dLock error", 2, "2018/9/1", "2018/10/1", "2019/7/31")]
        [TestCase("When Apprenticeship has been stopped  in the earning period then no dLock error", 2, "2018/9/1", "2017/9/1", "2018/9/1")]
        public void WhenApprenticeshipIsNotWithinCurrentDeliveryPeriodReturnNoDataLockErrors(string errorMessage, byte deliveryPeriod, DateTime periodStarDate,DateTime commitmentStarDate, DateTime commitmentEndDate)
        {

            var lastDayOfMonth = DateTime.DaysInMonth(periodStarDate.Year, periodStarDate.Month);
            var periodEndDate = new DateTime(periodStarDate.Year, periodStarDate.Month, lastDayOfMonth);
            
            calculatePeriodStartAndEndDate
                .Setup(x => x.GetPeriodDate(deliveryPeriod, academicYear))
                .Returns((periodStarDate, periodEndDate))
                .Verifiable();

            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { Identifier = PriceEpisodeIdentifier },
                EarningPeriod = new EarningPeriod{Period = deliveryPeriod },
                AcademicYear = academicYear,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
                    Ukprn = ukprn,
                    Uln = 200,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 100,
                            StartDate = commitmentStarDate
                        }
                    },
                    StopDate  = commitmentEndDate
                }
            };

            var validator = new MultipleLearnersValidator(dataLockLearnerCache.Object, calculatePeriodStartAndEndDate.Object);
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull(errorMessage);
            result.ApprenticeshipId.Should().Be(1, errorMessage);
            result.ApprenticeshipPriceEpisodes.Should().HaveCount(1, errorMessage);
            result.ApprenticeshipPriceEpisodes[0].Id.Should().Be(100, errorMessage);
        }

        [TestCase("When a duplicate start and end within apprenticeship return DLOCK_08", "2018/8/1", "2019/8/1", "2018/8/1", "2019/8/31")]
        [TestCase("When a duplicate start in middle of apprenticeship return DLOCK_08", "2018/8/1", "2019/8/1", "2018/11/1", "2019/8/31")]
        public void WhenDuplicateApprenticeshipsOverlapReturnDataLockErrors(string errorMessage, 
            DateTime duplicateApprenticeshipStartDate,
            DateTime duplicateApprenticeshipEndDate,
            DateTime apprenticeshipStarDate,
            DateTime apprenticeshipEndDate)
        {
            var earningPeriod = new EarningPeriod { Period = 5 };

            calculatePeriodStartAndEndDate
                .Setup(x => x.GetPeriodDate(earningPeriod.Period, academicYear))
                .Returns((DateTime.Parse("01 Dec 2018"), DateTime.Parse("31 Dec 2018")))
                .Verifiable();

            dataLockLearnerCache
                .Setup(x => x.GetDuplicateApprenticeships())
                .ReturnsAsync(new List<ApprenticeshipModel>
                {
                    new ApprenticeshipModel
                    {
                        Id = 2,
                        Uln = 200,
                        Ukprn = ukprn,
                        ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                        {
                            new ApprenticeshipPriceEpisodeModel
                            {
                                Id = 100,
                                StartDate = duplicateApprenticeshipStartDate
                            }
                        },
                        StopDate = duplicateApprenticeshipEndDate
                    }
                });

            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { Identifier = PriceEpisodeIdentifier },
                EarningPeriod = earningPeriod,
                AcademicYear = academicYear,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
                    Ukprn = ukprn,
                    Uln = 200,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 100,
                            StartDate = apprenticeshipStarDate
                        }
                    },
                    EstimatedEndDate = apprenticeshipEndDate
                }
            };

            var validator = new MultipleLearnersValidator(dataLockLearnerCache.Object, calculatePeriodStartAndEndDate.Object);
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().NotBeNull();
            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_08);
        }

    }
}
