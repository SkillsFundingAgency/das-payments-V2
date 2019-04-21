using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class StartDateValidatorTest
    {
        private DateTime startDate = DateTime.Today;
        private string priceEpisodeIdentifier = "pe-1";
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
        public void ReturnsNoDataLockErrors()
        {
            var apprenticeship = new ApprenticeshipModel
            {
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        StartDate = startDate.AddDays(-1)
                    }
                }
            };
            
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode {StartDate = startDate, Identifier = priceEpisodeIdentifier},
                EarningPeriod = period,
                Apprenticeship = apprenticeship
            };

            var validator = new StartDateValidator();

            var result = validator.Validate(validation);

            result.Any().Should().BeFalse();
        }

        [Test]
        public void ReturnsDataLockErrorWhenNotBegun()
        {
            var apprenticeship = new ApprenticeshipModel
            {
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                {
                    new ApprenticeshipPriceEpisodeModel
                    {
                        StartDate = startDate.AddDays(1)
                    }
                }
            };

            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { StartDate = startDate, Identifier = priceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = apprenticeship
            };

            var validator = new StartDateValidator();

            var result = validator.Validate(validation);

            result.Should().HaveCount(1);
            result.First().DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_09);
        }

    }
}