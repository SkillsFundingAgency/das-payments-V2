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
        private const string PriceEpisodeIdentifier = "pe-1";
        private EarningPeriod period;

        [SetUp]
        public void Setup()
        {
            period = new EarningPeriod
            {
                Period = 1
            };

            dataLockLearnerCache = new Mock<IDataLockLearnerCache>(MockBehavior.Strict);
        }


        [Test]
        public void WhenNoDuplicateApprenticeshipsReturnNoDataLockErrors()
        {
            long ukprn = 100;

            dataLockLearnerCache
                .Setup(x => x.GetDuplicateApprenticeships(ukprn))
                .ReturnsAsync(new List<ApprenticeshipModel>());

            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { Identifier = PriceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1 ,
                    Ukprn = ukprn,
                    Uln = 200,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 100
                        }
                    }
                }
            };

            var validator = new MultipleLearnersValidator(dataLockLearnerCache.Object);
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull();
            result.ApprenticeshipId.Should().Be(1);
            result.ApprenticeshipPriceEpisodes.Should().HaveCount(1);
            result.ApprenticeshipPriceEpisodes[0].Id.Should().Be(100);
        }

      

     
    }
}
