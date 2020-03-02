using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
using AutoFixture.NUnit3;
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
    public class OnProgrammeAndIncentiveStoppedValidatorTests
    {
        private const string PriceEpisodeIdentifier = "pe-1";
        private EarningPeriod period;
        private AutoMock mocker;
        private const short AcademicYear = 1920;

        [SetUp]
        public void Setup()
        {
            mocker = AutoMock.GetLoose();
            mocker.Provide<ICalculatePeriodStartAndEndDate, CalculatePeriodStartAndEndDate>();
            period = new EarningPeriod();

        }

        [AutoData]
        public void ReturnsOnlyCommitmentsThatAreNotStopped(ApprenticeshipModel apprenticeshipA, ApprenticeshipModel apprenticeshipB)
        {
            period.Period = 2;

            apprenticeshipA.StopDate = new DateTime(2019, 8, 1);
            apprenticeshipA.Status = ApprenticeshipStatus.Stopped;
            apprenticeshipA.ApprenticeshipPriceEpisodes[0].StartDate = new DateTime(2018, 8, 1);
            apprenticeshipA.ApprenticeshipPriceEpisodes[0].EndDate = new DateTime(2019, 8, 30);

            apprenticeshipB.Status = ApprenticeshipStatus.Active;
            apprenticeshipB.ApprenticeshipPriceEpisodes[0].StartDate = new DateTime(2019, 8, 1);
            apprenticeshipB.ApprenticeshipPriceEpisodes[0].EndDate = new DateTime(2020, 8, 30);

            var apprenticeships = new List<ApprenticeshipModel> { apprenticeshipA, apprenticeshipB };
            var validator = mocker.Create<OnProgrammeAndIncentiveStoppedValidator>();
            var result = validator.Validate(apprenticeships, TransactionType.Learning, period, AcademicYear);

            result.dataLockFailures.Should().BeEmpty();
            result.validApprenticeships.Should().NotBeNull();
            result.validApprenticeships.All(x => x.Id == apprenticeshipB.Id).Should().BeTrue();
        }

        [TestCase("When stop date is in a prior period then produce DLOCK_10", 1, "2019/7/31")]
        [TestCase("When stop date is mid-month then produce DLOCK_10", 1, "2019/8/20")]
        [TestCase("When stop date is in the middle of period 6 then produce DLOCK_10", 6, "2020/1/20")]
        [TestCase("When stop date is at the end of period 6 and we are testing period 7 then produce DLOCK_10", 7, "2020/1/31")]
        public void ScenariosThatProduceDLOCK_10(string errorMessage, byte testPeriod, DateTime commitmentStopDate)
        {
            period.Period = testPeriod;

            var apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 1,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 100,
                        },
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 200,
                            Removed = true
                        },
                    },
                    StopDate = commitmentStopDate,
                    Status = ApprenticeshipStatus.Stopped,
                }
            };

            var validator = mocker.Create<OnProgrammeAndIncentiveStoppedValidator>();

            var result = validator.Validate(apprenticeships, TransactionType.Learning, period, AcademicYear);
            result.dataLockFailures.Should().HaveCount(1, errorMessage);
            result.dataLockFailures[0].DataLockError.Should().Be(DataLockErrorCode.DLOCK_10);
            result.dataLockFailures[0].ApprenticeshipId.Should().Be(1);
            result.dataLockFailures[0].ApprenticeshipPriceEpisodeIds.Should().HaveCount(1);
            result.dataLockFailures[0].ApprenticeshipPriceEpisodeIds[0].Should().Be(100);

        }

        [TestCase("When stop date is the last day of the month then no datalock", 1, "2019/8/31")]
        [TestCase("When stop date occurs at the end of period 6 then no datalock", 6, "2020/1/31")]
        [TestCase("When stop date occurs in the middle of period 6 and we are testing period 5 then no datalock", 5, "2020/1/31")]
        [TestCase("When stop date is in a future period then no datalock", 1, "2020/8/31")]
        public void ScenariosThatDoNotProduceDatalocks(string errorMessage, byte testPeriod, DateTime commitmentStopDate)
        {
            period.Period = testPeriod;
            var apprenticeships = new List<ApprenticeshipModel> {
                new ApprenticeshipModel
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
                }
            };

            var validator = mocker.Create<OnProgrammeAndIncentiveStoppedValidator>();

            var result = validator.Validate(apprenticeships, TransactionType.Learning, period, AcademicYear);
            result.dataLockFailures.Should().BeEmpty(errorMessage);
            result.validApprenticeships[0].Id.Should().Be(1, errorMessage);
            result.validApprenticeships[0].ApprenticeshipPriceEpisodes[0].Id.Should().Be(100);
        }

        [TestCase("When stop date is in the middle of period 6 and transaction type is not Balancing then no DLOCK_10", 6, "2020/1/20",  TransactionType.Balancing)]
        [TestCase("When stop date is at the end of period 6 , we are testing period 7 and transaction type is Completion then no DLOCK_10", 7, "2020/1/31", TransactionType.Completion)]
        public void ScenariosThatDoNotProduceDatalocksBasedOnTransactionType(string errorMessage, byte testPeriod, DateTime commitmentStopDate, TransactionType type)
        {
            period.Period = testPeriod;
            var apprenticeships = new List<ApprenticeshipModel> {
                new ApprenticeshipModel
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
                }
            };

            var validator = mocker.Create<OnProgrammeAndIncentiveStoppedValidator>();
            var result = validator.Validate(apprenticeships, type, period, AcademicYear);

            result.dataLockFailures.Should().BeEmpty(errorMessage);
            result.validApprenticeships[0].Id.Should().Be(1, errorMessage);
            result.validApprenticeships[0].ApprenticeshipPriceEpisodes[0].Id.Should().Be(100);
        }



    }
}
