using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class NegotiatedPriceValidatorTest
    {
        private const decimal Price = 100m;
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
        public void GivenAgreedPriceMatchReturnNoDataLockErrors()
        {
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { AgreedPrice = Price, Identifier = PriceEpisodeIdentifier },
                EarningPeriod = period,
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel {Cost = Price + 10, Id = 97},
                    new ApprenticeshipPriceEpisodeModel{ Cost = Price, Id = 98},
                    new ApprenticeshipPriceEpisodeModel{Cost = Price + 20, Id = 99}
                }
            };

            var validator = new NegotiatedPriceValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull();
        }


        [Test]
        public void GivenAgreedPriceMatchReturnMatchingPriceEpisode()
        {
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { AgreedPrice = Price, Identifier = PriceEpisodeIdentifier },
                EarningPeriod = period,
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel {Cost = Price + 10, Id = 97},
                    new ApprenticeshipPriceEpisodeModel{ Cost = Price, Id = 98},
                    new ApprenticeshipPriceEpisodeModel{Cost = Price + 20, Id = 99}
                }
            };

            var validator = new NegotiatedPriceValidator();
            var result = validator.Validate(validation);
            result.ApprenticeshipPriceEpisodes.Any(ape => ape.Id == 98).Should().BeTrue();
        }

        [Test]
        public void GivenAgreedPriceDoNotMatchReturnDataLockError07()
        {

            var apprenticeship = new ApprenticeshipModel
            {
            };

            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { AgreedPrice = Price, Identifier = PriceEpisodeIdentifier },
                EarningPeriod = period,
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        Cost = 200
                    }
                }
            };

            var validator = new NegotiatedPriceValidator();

            var result = validator.Validate(validation);

            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_07);
        }
    }
}