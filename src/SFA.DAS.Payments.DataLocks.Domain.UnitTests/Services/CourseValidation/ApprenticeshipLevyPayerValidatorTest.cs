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
    public class ApprenticeshipLevyPayerValidatorTest
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
        public void WhenApprenticeshipEmployerIsALevyPayerReturnNoDataLockErrors()
        {
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { Identifier = PriceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1 ,
                    IsLevyPayer =  true,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 100
                        }
                    }
                }
            };

            var validator = new ApprenticeshipLevyPayerValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull();
            result.ApprenticeshipPriceEpisodes.Should().HaveCount(1);
            result.ApprenticeshipPriceEpisodes[0].Id.Should().Be(100);
        }

        [Test]
        public void GivenApprenticeshipEmployerIsNotALevyPayerReturnDataLock_11()
        {
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode { Identifier = PriceEpisodeIdentifier },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
                    IsLevyPayer = false,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel()
                    }
                }
            };

            var validator = new ApprenticeshipLevyPayerValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().Be(DataLockErrorCode.DLOCK_11);
            result.ApprenticeshipPriceEpisodes.Should().BeEmpty();
        }

        [Test]
        public void WhenApprenticeshipEmployerNotIsALevyPayerButTransferReceiverReturnNoDataLockErrors()
        {
            var validation = new DataLockValidationModel
            {
                PriceEpisode = new PriceEpisode
                {
                    Identifier = PriceEpisodeIdentifier
                },
                EarningPeriod = period,
                Apprenticeship = new ApprenticeshipModel
                {
                    Id = 1,
                    IsLevyPayer = false,
                    AccountId = 50,
                    TransferSendingEmployerAccountId = 100,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 100
                        }
                    }
                }
            };

            var validator = new ApprenticeshipLevyPayerValidator();
            var result = validator.Validate(validation);
            result.DataLockErrorCode.Should().BeNull();
            result.ApprenticeshipPriceEpisodes.Should().HaveCount(1);
            result.ApprenticeshipPriceEpisodes[0].Id.Should().Be(100);
        }


    }
}
