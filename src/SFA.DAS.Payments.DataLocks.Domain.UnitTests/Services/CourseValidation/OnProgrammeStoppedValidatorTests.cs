using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class OnProgrammeStoppedValidatorTests
    {
        private const string PriceEpisodeIdentifier = "pe-1";
        private EarningPeriod period;
        private Mock<ICalculatePeriodStartAndEndDate> calculatePeriodStartAndEndDate;
        private const short AcademicYear = 1920;

        [SetUp]
        public void Setup()
        {
            period = new EarningPeriod();
            calculatePeriodStartAndEndDate = new Mock<ICalculatePeriodStartAndEndDate>(MockBehavior.Strict);
        }

        [TearDown]
        public void CleanUp()
        {
            calculatePeriodStartAndEndDate.Verify();
        }

        [TestCase("When stop date is in a prior period then produce DLOCK_10", 1, "2019/7/31", "2019/8/31")]
        [TestCase("When stop date is mid-month then produce DLOCK_10", 1, "2019/8/20", "2019/8/31")]
        [TestCase("When stop date is in the middle of period 6 then produce DLOCK_10", 6, "2020/1/20", "2020/1/31")]
        [TestCase("When stop date is at the end of period 6 and we are testing period 7 then produce DLOCK_10", 7, "2020/1/31", "2020/2/29")]
        public void ScenariosThatProduceDLOCK_10(string errorMessage, byte testPeriod, DateTime commitmentStopDate, DateTime periodEndDate)
        {
            period.Period = testPeriod;
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { Identifier = PriceEpisodeIdentifier, },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1 ,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 100,
                        },
                    },
                    StopDate = commitmentStopDate,
                    Status = ApprenticeshipStatus.Stopped,
                },
                AcademicYear = AcademicYear,
                TransactionType = TransactionType.Learning,
            };

            calculatePeriodStartAndEndDate
                .Setup(x => x.GetPeriodDate(testPeriod, AcademicYear))
                .Returns((new DateTime(periodEndDate.Year, periodEndDate.Month, 1), periodEndDate))
                .Verifiable();

            var validator = new OnProgrammeStoppedValidator(calculatePeriodStartAndEndDate.Object);
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_10, errorMessage);
            result.ApprenticeshipPriceEpisodes.Should().BeEmpty(errorMessage);
        }

        [TestCase("When stop date is the last day of the month then no datalock", 1, "2019/8/31", "2019/8/31")]
        [TestCase("When stop date occurs at the end of period 6 then no datalock", 6, "2020/1/31", "2020/1/31")]
        [TestCase("When stop date occurs in the middle of period 6 and we are testing period 5 then no datalock", 5, "2020/1/31", "2019/12/31")]
        [TestCase("When stop date is in a future period then no datalock", 1, "2020/8/31", "2019/8/31")]
        public void ScenariosThatDoNotProduceDatalocks(string errorMessage, byte testPeriod, DateTime commitmentStopDate,DateTime periodEndDate)
        {
            period.Period = testPeriod;
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { Identifier = PriceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
                    Status = ApprenticeshipStatus.Stopped,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 100,
                        },
                    },
                    StopDate = commitmentStopDate, 
                },
                AcademicYear = AcademicYear,
                TransactionType = TransactionType.Learning,
            };

            calculatePeriodStartAndEndDate
                .Setup(x => x.GetPeriodDate(testPeriod, AcademicYear))
                .Returns((new DateTime(periodEndDate.Year, periodEndDate.Month, 1), periodEndDate))
                .Verifiable();


            var validator = new OnProgrammeStoppedValidator(calculatePeriodStartAndEndDate.Object);
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull(errorMessage);
            result.ApprenticeshipPriceEpisodes.Should().HaveCount(1, errorMessage);
            result.ApprenticeshipPriceEpisodes[0].Id.Should().Be(100, errorMessage);
        }

        [TestCase("When stop date is in the middle of period 6 and transaction type is not OnProgramme then no DLOCK_10", 6, "2020/1/20", "2020/1/31", TransactionType.First16To18ProviderIncentive)]
        [TestCase("When stop date is at the end of period 6 , we are testing period 7 and transaction type is OnProgramme then no DLOCK_10", 7, "2020/1/31", "2020/2/29", TransactionType.LearningSupport)]
        public void ScenariosThatDoNotProduceDatalocksBasedOnTransactionType(string errorMessage, byte testPeriod, DateTime commitmentStopDate, DateTime periodEndDate, TransactionType type)
        {
            period.Period = testPeriod;
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { Identifier = PriceEpisodeIdentifier, },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 100,
                        },
                    },
                    StopDate = commitmentStopDate,
                    Status = ApprenticeshipStatus.Stopped,
                },
                AcademicYear = AcademicYear,
                TransactionType = type
            };

            var validator = new OnProgrammeStoppedValidator(calculatePeriodStartAndEndDate.Object);
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull(errorMessage);
            result.ApprenticeshipPriceEpisodes.Should().HaveCount(1, errorMessage);
            result.ApprenticeshipPriceEpisodes[0].Id.Should().Be(100, errorMessage);
        }



    }
}
