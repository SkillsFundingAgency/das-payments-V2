using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services
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
            var apprenticeship = new ApprenticeshipModel
            {
                Status = ApprenticeshipPaymentStatus.Active,
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        Id = 1
                    }
                }
            };

            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { Identifier = PriceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = apprenticeship
            };

            var validator = new ApprenticeshipPauseValidator();
            var result = validator.Validate(validation);

            result.Any().Should().BeFalse();
        }

        [Test]
        public void GivenStatusIsPausedReturnDataLockErrors()
        {
            var apprenticeship = new ApprenticeshipModel
            {
                Status = ApprenticeshipPaymentStatus.Paused,
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        Id = 1
                    },
                    new ApprenticeshipPriceEpisodeModel
                    {
                        Id = 2
                    }
                }
            };
           
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { Identifier = PriceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = apprenticeship
            };

            var validator = new ApprenticeshipPauseValidator();

            var result = validator.Validate(validation);

            result.Should().HaveCount(2);
            result[0].DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_12);
            result[0].ApprenticeshipPriceEpisodeIdentifier.Should().Be(1);
            result[1].DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_12);
            result[1].ApprenticeshipPriceEpisodeIdentifier.Should().Be(2);
        }

    }
}
