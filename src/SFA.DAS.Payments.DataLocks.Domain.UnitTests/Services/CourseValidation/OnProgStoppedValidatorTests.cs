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
            period = new EarningPeriod();
        }


        [Test]
        public void WhenStopDateIsInAPriorPeriod_ThenDLOCK_10()
        {
            period.Period = 1;
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
        public void WhenStopDateIsMidMonth_ThenDLOCK_10()
        {
            period.Period = 1;
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
                    StopDate = new DateTime(2019, 08, 20), 
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
        public void WhenStopDateIsTheLastDayOfTheMonth_ThenNoDatalock()
        {
            period.Period = 1;
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
                    StopDate = new DateTime(2019, 08, 31), 
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

        [Test]
        public void WhenStopDateOccursInTheMiddleOfPeriod6_ThenDLOCK_10()
        {
            period.Period = 6;
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
                    StopDate = new DateTime(2020, 01, 20), 
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
        public void WhenStopDateOccursAtTheEndOfPeriod6AndWeAreTestingPeriod7_ThenDLOCK_10()
        {
            period.Period = 7;
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
                    StopDate = new DateTime(2020, 01, 31), 
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
        public void WhenStopDateOccursAtTheEndOfPeriod6_ThenNoDatalock()
        {
            period.Period = 6;
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
                    StopDate = new DateTime(2020, 01, 31), 
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

        [Test]
        public void WhenStopDateOccursInTheMiddleOfPeriod6AndWeAreTestingPeriod5_ThenNoDatalock()
        {
            period.Period = 5;
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
                    StopDate = new DateTime(2020, 01, 31), 
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

        [Test]
        public void WhenStopDateIsInAFuturePeriod_ThenNoDatalock()
        {
            period.Period = 1;
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
                    StopDate = new DateTime(2020, 08, 01), 
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
