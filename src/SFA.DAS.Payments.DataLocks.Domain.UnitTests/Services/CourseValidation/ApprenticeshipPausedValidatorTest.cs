using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class ApprenticeshipPausedValidatorTest
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
        public void WhenStatusIsNotPauseReturnNoDataLockErrors()
        {
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { Identifier = PriceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1 ,
                    ApprenticeshipPriceEpisodes =new List<ApprenticeshipPriceEpisodeModel>()
                }
            };

            var validator = new ApprenticeshipPauseValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull();
        }

        [Test]
        public void GivenStatusIsPausedReturnDataLockErrors()
        {
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { Identifier = PriceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
                    Status = ApprenticeshipStatus.Paused,
                }
            };

            var validator = new ApprenticeshipPauseValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().NotBeNull();
            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_12);
        }
    }
}
