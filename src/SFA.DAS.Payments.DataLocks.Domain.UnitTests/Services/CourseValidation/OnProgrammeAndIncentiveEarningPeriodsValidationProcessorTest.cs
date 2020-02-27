using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Extras.Moq;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class OnProgrammeAndIncentiveEarningPeriodsValidationProcessorTest
    {
        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
        }

        [Test]
        public void GivenNoDLockMatchedPriceEpisodeIsReturnedInValidPeriods()
        {
            var apprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();

            var earningPeriods = new List<EarningPeriod>{ earningEvent.OnProgrammeEarnings[0].Periods[0]};

            mocker.Mock<IStartDateValidator>()
                    .Setup(x => x.Validate(It.IsAny<PriceEpisode>(), It.IsAny<List<ApprenticeshipModel>>()))
                    .Returns((apprenticeships, new List<DataLockFailure>()));

            mocker.Mock<IOnProgrammeAndIncentiveStoppedValidator>()
                .Setup(x => x.Validate(
                    It.IsAny<List<ApprenticeshipModel>>(),
                    It.IsAny<TransactionType>(),
                    It.IsAny<EarningPeriod>(), It.IsAny<int>()))
                .Returns((apprenticeships, new List<DataLockFailure>()));

            mocker.Mock<ICompletionStoppedValidator>()
                .Setup(x => x.Validate(
                    It.IsAny<PriceEpisode>(),
                    It.IsAny<List<ApprenticeshipModel>>(),
                    It.IsAny<TransactionType>()))
                .Returns((apprenticeships, new List<DataLockFailure>()));

            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(
                    It.Is<DataLockValidationModel>(o => o.Apprenticeship.Id == apprenticeships[0].Id)))
                .Returns(() => new CourseValidationResult
                {
                    MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel
                    {
                        Id = apprenticeships[0].ApprenticeshipPriceEpisodes[0].Id
                    },
                    DataLockFailures = new List<DataLockFailure>()
                });

            var earningProcessor = mocker.Create<OnProgrammeAndIncentiveEarningPeriodsValidationProcessor>();

            var periods = earningProcessor.ValidatePeriods(
                earningEvent.Ukprn,
                earningEvent.Learner.Uln,
                earningEvent.PriceEpisodes,
                earningPeriods,
                (TransactionType)earningEvent.OnProgrammeEarnings[0].Type,
                apprenticeships,
                earningEvent.LearningAim,
                earningEvent.CollectionPeriod.AcademicYear);

            periods.ValidPeriods.Count.Should().Be(1);
            periods.ValidPeriods.All(p => p.ApprenticeshipPriceEpisodeId == apprenticeships[0].ApprenticeshipPriceEpisodes[0].Id)
                .Should().Be(true);
        }

        [Test]
        public void GivenThereIsStartDateDLock09NoOtherDLockShouldBeGenerated()
        {
            var allApprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();

            var apprenticeships = new List<ApprenticeshipModel>
            {
                allApprenticeships .First()
            };

            mocker.Mock<IStartDateValidator>()
                .Setup(x => x.Validate(It.IsAny<PriceEpisode>(), It.IsAny<List<ApprenticeshipModel>>()))
                .Returns((new List<ApprenticeshipModel>(), new List<DataLockFailure>
                {
                    new DataLockFailure
                    {
                        ApprenticeshipPriceEpisodeIds = apprenticeships[0].ApprenticeshipPriceEpisodes.Select(x => x.Id).ToList(),
                        ApprenticeshipId = apprenticeships[0].Id,
                        DataLockError = DataLockErrorCode.DLOCK_09
                    }
                }));

            mocker.Mock<IOnProgrammeAndIncentiveStoppedValidator>()
                .Setup(x => x.Validate(
                    It.IsAny<List<ApprenticeshipModel>>(),
                    It.IsAny<TransactionType>(),
                    It.IsAny<EarningPeriod>(), It.IsAny<int>()))
                .Returns((new List<ApprenticeshipModel>(), new List<DataLockFailure>
                {
                    new DataLockFailure
                    {
                        ApprenticeshipPriceEpisodeIds = apprenticeships[0].ApprenticeshipPriceEpisodes.Select(x => x.Id).ToList(),
                        ApprenticeshipId = apprenticeships[0].Id,
                        DataLockError = DataLockErrorCode.DLOCK_10
                    }
                }));

            mocker.Mock<ICompletionStoppedValidator>()
                .Setup(x => x.Validate(
                    It.IsAny<PriceEpisode>(),
                    It.IsAny<List<ApprenticeshipModel>>(),
                    It.IsAny<TransactionType>()))
                .Returns((apprenticeships, new List<DataLockFailure>()));

            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)))
                .Returns(() => new CourseValidationResult
                {
                    MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel
                    {
                        Id = apprenticeships[0].ApprenticeshipPriceEpisodes[0].Id
                    }
                });

            var earningProcessor = mocker.Create<OnProgrammeAndIncentiveEarningPeriodsValidationProcessor>();

            var periods = earningProcessor.ValidatePeriods(
                earningEvent.Ukprn,
                1,
                earningEvent.PriceEpisodes,
                earningEvent.OnProgrammeEarnings[0].Periods.ToList(),
                (TransactionType)earningEvent.OnProgrammeEarnings[0].Type,
                apprenticeships,
                earningEvent.LearningAim,
                earningEvent.CollectionPeriod.AcademicYear);

            periods.ValidPeriods.Should().BeEmpty();
            periods.InValidPeriods.All(p => p.DataLockFailures.All(x =>
                x.ApprenticeshipId == apprenticeships[0].Id &&
                x.DataLockError == DataLockErrorCode.DLOCK_09 &&
                x.ApprenticeshipPriceEpisodeIds.All(o => apprenticeships[0].ApprenticeshipPriceEpisodes.Select(a => a.Id).Contains(o)))
            ).Should().BeTrue();
        }

        [Test]
        public void GivenThereIsCompletionStoppedDLock10NoOtherDLockShouldBeGenerated()
        {
            var allApprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();

            var apprenticeships = new List<ApprenticeshipModel>
            {
                allApprenticeships .First()
            };

            earningEvent.OnProgrammeEarnings[0].Type = OnProgrammeEarningType.Completion;

            mocker.Mock<IStartDateValidator>()
                .Setup(x => x.Validate(It.IsAny<PriceEpisode>(), It.IsAny<List<ApprenticeshipModel>>()))
                .Returns((apprenticeships, new List<DataLockFailure>()));

            mocker.Mock<IOnProgrammeAndIncentiveStoppedValidator>()
                .Setup(x => x.Validate(
                    It.IsAny<List<ApprenticeshipModel>>(),
                    It.IsAny<TransactionType>(),
                    It.IsAny<EarningPeriod>(), It.IsAny<int>()))
                .Returns((apprenticeships, new List<DataLockFailure>()));

            mocker.Mock<ICompletionStoppedValidator>()
                .Setup(x => x.Validate(
                    It.IsAny<PriceEpisode>(),
                    It.IsAny<List<ApprenticeshipModel>>(),
                    It.IsAny<TransactionType>()))
                .Returns((new List<ApprenticeshipModel>(), new List<DataLockFailure>
                {
                    new DataLockFailure
                    {
                        ApprenticeshipPriceEpisodeIds = apprenticeships[0].ApprenticeshipPriceEpisodes.Select(x => x.Id).ToList(),
                        ApprenticeshipId = apprenticeships[0].Id,
                        DataLockError = DataLockErrorCode.DLOCK_10
                    }
                }));

            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)))
                .Returns(() => new CourseValidationResult
                {
                    DataLockFailures = new List<DataLockFailure>
                    {
                        new DataLockFailure
                        {
                            ApprenticeshipPriceEpisodeIds = apprenticeships[0].ApprenticeshipPriceEpisodes.Select(x => x.Id).ToList(),
                            ApprenticeshipId = apprenticeships[0].Id,
                            DataLockError = DataLockErrorCode.DLOCK_07
                        }
                    }
                });

            var earningProcessor = mocker.Create<OnProgrammeAndIncentiveEarningPeriodsValidationProcessor>();
            var periods = earningProcessor.ValidatePeriods(
                earningEvent.Ukprn,
                earningEvent.Learner.Uln,
                earningEvent.PriceEpisodes,
                earningEvent.OnProgrammeEarnings[0].Periods.ToList(),
                (TransactionType)earningEvent.OnProgrammeEarnings[0].Type,
                apprenticeships,
                earningEvent.LearningAim,
                earningEvent.CollectionPeriod.AcademicYear);

            periods.ValidPeriods.Should().BeEmpty();
            periods.InValidPeriods.All(p => p.DataLockFailures.All(x =>
                x.ApprenticeshipId == apprenticeships[0].Id &&
                x.DataLockError == DataLockErrorCode.DLOCK_10 &&
                x.ApprenticeshipPriceEpisodeIds.All(o => apprenticeships[0].ApprenticeshipPriceEpisodes.Select(a => a.Id).Contains(o)))
            ).Should().BeTrue();

        }

        [Test]
        public void GivenThereIsOnProgrammeAndIncentiveStoppedDLock10NoOtherDLockShouldBeGenerated()
        {
            var apprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();
            earningEvent.OnProgrammeEarnings[0].Type = OnProgrammeEarningType.Learning;

            mocker.Mock<IStartDateValidator>()
                .Setup(x => x.Validate(It.IsAny<PriceEpisode>(), It.IsAny<List<ApprenticeshipModel>>()))
                .Returns((apprenticeships, new List<DataLockFailure>()));

            mocker.Mock<IOnProgrammeAndIncentiveStoppedValidator>()
                .Setup(x => x.Validate(
                    It.IsAny<List<ApprenticeshipModel>>(),
                    It.IsAny<TransactionType>(),
                    It.IsAny<EarningPeriod>(), It.IsAny<int>()))
                .Returns((new List<ApprenticeshipModel>(), new List<DataLockFailure>
                {
                    new DataLockFailure
                    {
                        ApprenticeshipPriceEpisodeIds = apprenticeships[0].ApprenticeshipPriceEpisodes.Select(x => x.Id).ToList(),
                        ApprenticeshipId = apprenticeships[0].Id,
                        DataLockError = DataLockErrorCode.DLOCK_10
                    }
                }));

            mocker.Mock<ICompletionStoppedValidator>()
                .Setup(x => x.Validate(
                    It.IsAny<PriceEpisode>(),
                    It.IsAny<List<ApprenticeshipModel>>(),
                    It.IsAny<TransactionType>()))
                .Returns((apprenticeships, new List<DataLockFailure>()));

            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)))
                .Returns(() => new CourseValidationResult
                {
                    DataLockFailures = new List<DataLockFailure>
                    {
                        new DataLockFailure
                        {
                            ApprenticeshipPriceEpisodeIds = apprenticeships[0].ApprenticeshipPriceEpisodes.Select(x => x.Id).ToList(),
                            ApprenticeshipId = apprenticeships[0].Id,
                            DataLockError = DataLockErrorCode.DLOCK_07
                        }
                    }
                });

            var earningProcessor = mocker.Create<OnProgrammeAndIncentiveEarningPeriodsValidationProcessor>();
            var periods = earningProcessor.ValidatePeriods(
                earningEvent.Ukprn,
                earningEvent.Learner.Uln,
                earningEvent.PriceEpisodes,
                earningEvent.OnProgrammeEarnings[0].Periods.ToList(),
                (TransactionType)earningEvent.OnProgrammeEarnings[0].Type,
                apprenticeships,
                earningEvent.LearningAim,
                earningEvent.CollectionPeriod.AcademicYear);
            
            periods.ValidPeriods.Should().BeEmpty();
            periods.InValidPeriods.All(p => p.DataLockFailures.All(x =>
                x.ApprenticeshipId == apprenticeships[0].Id &&
                x.DataLockError == DataLockErrorCode.DLOCK_10 &&
                x.ApprenticeshipPriceEpisodeIds.All(o => apprenticeships[0].ApprenticeshipPriceEpisodes.Select(a => a.Id).Contains(o)))
            ).Should().BeTrue();

        }

        [Test]
        public void OnlyValidateCommitmentsThatStartedBeforePriceEpisodes()
        {
            var apprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();

            earningEvent.PriceEpisodes[0].EffectiveTotalNegotiatedPriceStartDate = new DateTime(2018, 8, 30);



            var earningProcessor = new OnProgrammeAndIncentiveEarningPeriodsValidationProcessor(
                new StartDateValidator(),
                new OnProgrammeAndIncentiveStoppedValidator(new CalculatePeriodStartAndEndDate()),
                new CompletionStoppedValidator(),
                new CourseValidationProcessor(new List<ICourseValidator> { new StandardCodeValidator() })
            );

            var periods = earningProcessor.ValidatePeriods(
                earningEvent.Ukprn,
                earningEvent.Learner.Uln,
                earningEvent.PriceEpisodes,
                earningEvent.OnProgrammeEarnings[0].Periods.ToList(),
                (TransactionType)earningEvent.OnProgrammeEarnings[0].Type,
                apprenticeships,
                earningEvent.LearningAim,
                earningEvent.CollectionPeriod.AcademicYear);

            periods.ValidPeriods.Count.Should().Be(2);
            periods.ValidPeriods.All(x => x.ApprenticeshipId == 1).Should().BeTrue();
        }

        private ApprenticeshipContractType1EarningEvent CreateEarningEventTestData()
        {
            return new ApprenticeshipContractType1EarningEvent
            {
                Learner = new Learner
                {
                    Uln = 1
                },
                LearningAim = new LearningAim
                {
                    FrameworkCode = 490,
                    PathwayCode = 1,
                    ProgrammeType = 3,
                    StandardCode = 0
                },
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = 1920,
                    Period = 1
                },
                PriceEpisodes = new List<PriceEpisode>
                {
                    new PriceEpisode
                    {
                        Identifier = "pe-1",
                        TotalNegotiatedPrice1 = 2000m,
                        AgreedPrice = 2000m,
                        EffectiveTotalNegotiatedPriceStartDate = new DateTime(2018,8,30),
                        PlannedEndDate =  new DateTime(2019,8,30),
                        ActualEndDate = new DateTime(2019,9,2),
                    }
                },
                OnProgrammeEarnings = new List<OnProgrammeEarning>
                {
                        new OnProgrammeEarning
                        {
                            Type = OnProgrammeEarningType.Completion,
                            Periods = new List<EarningPeriod>
                            {
                                new EarningPeriod
                                {
                                    Amount = 1,
                                    PriceEpisodeIdentifier = "pe-1",
                                    Period = 1
                                },
                                new EarningPeriod
                                {
                                    Amount = 2,
                                    PriceEpisodeIdentifier = "pe-1",
                                    Period = 2
                                },
                            }.AsReadOnly()
                        }
              }
            };
        }

        private List<ApprenticeshipModel> CreateApprenticeships()
        {
            return new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 1,
                    Uln = 1,
                    AccountId = 21,
                    Ukprn = 1,
                    EstimatedStartDate = new DateTime(2018, 8, 1),
                    EstimatedEndDate = new DateTime(2019, 8, 1),
                    Status = ApprenticeshipStatus.Active,
                    PathwayCode = 490,
                    FrameworkCode = 1,
                    ProgrammeType = 3,
                    StandardCode = 0,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 1,
                            Cost = 2000,
                            StartDate = new DateTime(2018, 8, 1),
                            EndDate = new DateTime(2019, 8, 1),
                        }
                    },
                },
                new ApprenticeshipModel
                {
                    Id = 2,
                    Uln = 1,
                    AccountId = 21,
                    Ukprn = 1,
                    EstimatedStartDate = new DateTime(2019, 9, 1),
                    EstimatedEndDate = new DateTime(2020, 10, 1),
                    Status = ApprenticeshipStatus.Active,
                    StandardCode = 196,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel {
                            Id = 1,
                            Cost = 5000,
                            StartDate = new DateTime(2019, 9, 1),
                            EndDate = new DateTime(2020, 10, 1)
                        }
                    },
                },
            };
        }

    }
}
