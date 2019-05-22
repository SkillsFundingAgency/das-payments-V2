using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class OnProgrammePeriodsValidationProcessorTests
    {
        private AutoMock mocker;
        private List<PriceEpisode> priceEpisodes;
        private List<ApprenticeshipModel> apprenticeships;
        private List<DataLockFailure> dataLockFailures;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            priceEpisodes = new List<PriceEpisode>
            {
                new PriceEpisode{Identifier = "pe-1"},
                new PriceEpisode{Identifier = "pe-2"},
                new PriceEpisode{Identifier = "pe-3"}
            };
            apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 1,
                    AccountId = 21,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 90},
                        new ApprenticeshipPriceEpisodeModel{Id = 91},
                        new ApprenticeshipPriceEpisodeModel{Id = 92},
                        new ApprenticeshipPriceEpisodeModel{Id = 93},
                    }
                },
                new ApprenticeshipModel
                {
                    Id = 2,
                    AccountId = 22,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 94},
                        new ApprenticeshipPriceEpisodeModel{Id = 95},
                        new ApprenticeshipPriceEpisodeModel{Id = 96},
                        new ApprenticeshipPriceEpisodeModel{Id = 97},
                    }
                }

            };

            dataLockFailures = new List<DataLockFailure>
            {
                new DataLockFailure
                {
                    ApprenticeshipPriceEpisodeIds = new List<long>(),
                    DataLockError = DataLockErrorCode.DLOCK_03
                }
            };
        }

        [Test]
        public void MatchedPriceEpisodeIsReturnedInValidPeriods()
        {
            var earning = new OnProgrammeEarning
            {
                Periods = new List<EarningPeriod> { new EarningPeriod { Amount = 1, PriceEpisodeIdentifier = "pe-1" } }.AsReadOnly()
            };
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)))
                .Returns(() => new CourseValidationResult { MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 90 } });
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 2)))
                .Returns(() => new CourseValidationResult { DataLockFailures = dataLockFailures });

            var periods = mocker.Create<OnProgrammePeriodsValidationProcessor>().ValidatePeriods(1, priceEpisodes, earning, apprenticeships);
            periods.ValidPeriods.Count.Should().Be(1);
            periods.ValidPeriods.All(p => p.ApprenticeshipPriceEpisodeId == 90).Should().Be(true);
        }

        [Test]
        public void MatchesApprenticeship()
        {
            var earning = new OnProgrammeEarning
            {
                Periods = new List<EarningPeriod> { new EarningPeriod
                {
                    Amount = 1,
                    PriceEpisodeIdentifier = "pe-1",
                } }.AsReadOnly()
            };
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)))
                .Returns(() => new CourseValidationResult { MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 90 } });
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 2)))
                .Returns(() => new CourseValidationResult { MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 96 } });


            var periods = mocker.Create<OnProgrammePeriodsValidationProcessor>().ValidatePeriods(1, priceEpisodes,
                earning, apprenticeships);
            periods.ValidPeriods.Count.Should().Be(2);
            periods.ValidPeriods.Any(p =>  p.ApprenticeshipPriceEpisodeId == 90 && p.AccountId == 21).Should().Be(true);
            periods.ValidPeriods.Any(p =>  p.ApprenticeshipPriceEpisodeId == 96 && p.AccountId == 22).Should().Be(true);

        }

        [Test]
        public void DataLocksAreReturnedInInvalidPeriods()
        {
            var earning = new OnProgrammeEarning
            {
                Periods = new List<EarningPeriod> { new EarningPeriod { Amount = 1, PriceEpisodeIdentifier = "pe-1" } }.AsReadOnly()
            };
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)))
                .Returns(() => new CourseValidationResult { MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 90 } });
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 2)))
                .Returns(() => new CourseValidationResult { DataLockFailures = dataLockFailures });

            var periods = mocker.Create<OnProgrammePeriodsValidationProcessor>().ValidatePeriods(1, priceEpisodes,
                earning, apprenticeships);
            periods.InValidPeriods.Count.Should().Be(1);
            periods.InValidPeriods.All(p => p.DataLockFailures[0].DataLockError == DataLockErrorCode.DLOCK_03).Should().BeTrue();
        }

        [Test]
        public void ZeroAmountPeriodsAreAddedToValidPeriod()
        {
            var earning = new OnProgrammeEarning
            {
                Periods = new List<EarningPeriod> { new EarningPeriod { Amount = 0, PriceEpisodeIdentifier = "pe-1" } }.AsReadOnly()
            };
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)))
                .Returns(() => new CourseValidationResult { MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 90 } });
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 2)))
                .Returns(() => new CourseValidationResult { DataLockFailures = dataLockFailures });

            var periods = mocker.Create<OnProgrammePeriodsValidationProcessor>().ValidatePeriods(1, priceEpisodes,
                earning, apprenticeships);
            periods.ValidPeriods.Count.Should().Be(1);
        }

        [Test]
        public void PopulatesTransferSenderAccountIdForMatchedApprenticeship()
        {
            var earning = new OnProgrammeEarning
            {
                Periods = new List<EarningPeriod> { new EarningPeriod
                {
                    Amount = 1,
                    PriceEpisodeIdentifier = "pe-1",
                } }.AsReadOnly()
            };
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)))
                .Returns(() => new CourseValidationResult { MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 90 } });
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 2)))
                .Returns(() => new CourseValidationResult { MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 96 } });

            apprenticeships.FirstOrDefault().TransferSendingEmployerAccountId = 999;
            var periods = mocker.Create<OnProgrammePeriodsValidationProcessor>().ValidatePeriods(1, priceEpisodes, earning, apprenticeships);
            periods.ValidPeriods.Count.Should().Be(2);
            periods.ValidPeriods.Any(p => p.Apprenticeship.Id == 1 && p.Period.TransferSenderAccountId == 999).Should().Be(true);
        }
    }
}