using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class OnProgStoppedValidatorTests
    {
        private const string PriceEpisodeIdentifier = "pe-1";
        private EarningPeriod period;

        [OneTimeSetUp]
        public void Setup()
        {
            period = new EarningPeriod
            {
                Period = 1
            };
        }


        [Test]
        public void WhenStopDateIsInAPriorPeriod_ThenNoDatalock()
        {
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
                    StopDate = new DateTime(2019,07,31), // Last day of 1819
                    Status = ApprenticeshipStatus.Stopped,
                },
                AcademicYear = 1920,
                TransactionType = OnProgrammeEarningType.Learning,
            };

            var validator = new OnProgStoppedValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_10);
            result.ApprenticeshipPriceEpisodes.Should().BeEmpty();
        }

        [Test]
        public void WhenStopDateIsInAFuturePeriod_ThenNoDatalock()
        {
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
                    StopDate = new DateTime(2020, 08, 01), // First day of next academic year
                },
                AcademicYear = 1920,
                TransactionType = OnProgrammeEarningType.Learning,
            };

            var validator = new OnProgStoppedValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull();
            result.ApprenticeshipPriceEpisodes.Should().HaveCount(1);
            result.ApprenticeshipPriceEpisodes[0].Id.Should().Be(100);
        }
    }
}
