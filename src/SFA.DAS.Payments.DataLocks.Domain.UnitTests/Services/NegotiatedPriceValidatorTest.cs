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
    public class NegotiatedPriceValidatorTest
    {
        private const decimal Price = 100m;
        private const string PriceEpisodeIdentifier = "pe-1";
        private const byte Period = 1;

        [Test]
        public void GivenAgreedPriceMatchReturnNoDataLockErrors()
        {
            var apprenticeship = new ApprenticeshipModel
            {
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel {Cost = Price + 10},
                    new ApprenticeshipPriceEpisodeModel{ Cost = Price},
                    new ApprenticeshipPriceEpisodeModel{Cost = Price + 20}
                }
            };

            var apprenticeships = new List<ApprenticeshipModel> { apprenticeship };

            var validation = new CourseValidationModel
            {
                PriceEpisode = new PriceEpisode { AgreedPrice = Price, Identifier = PriceEpisodeIdentifier },
                Period = Period,
                Apprenticeships = apprenticeships
            };

            var validator = new NegotiatedPriceValidator();
            var result = validator.Validate(validation);
            result.Any().Should().BeFalse();
        }

        [Test]
        public void GivenAgreedPriceDoNotMatchReturnDataLockError07()
        {

            var apprenticeship = new ApprenticeshipModel
            {
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                       Cost = 200
                    }
                }
            };

            var apprenticeships = new List<ApprenticeshipModel> { apprenticeship };

            var validation = new CourseValidationModel
            {
                PriceEpisode = new PriceEpisode { AgreedPrice = Price, Identifier = PriceEpisodeIdentifier },
                Period = Period,
                Apprenticeships = apprenticeships
            };

            var validator = new NegotiatedPriceValidator();

            var result = validator.Validate(validation);

            result.Should().HaveCount(1);
            result.First().DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_07);
        }
    }
}