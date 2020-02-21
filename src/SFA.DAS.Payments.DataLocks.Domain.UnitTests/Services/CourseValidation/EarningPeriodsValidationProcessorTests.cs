using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class EarningPeriodsValidationProcessorTests
    {
        private AutoMock mocker;
        private List<PriceEpisode> priceEpisodes;
        private List<ApprenticeshipModel> apprenticeships;
        private List<DataLockFailure> dataLockFailures;
        private LearningAim aim;
        private int AcademicYear = 1819;
        private readonly int ukprn = 123;

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

            dataLockFailures = new List<DataLockFailure>
            {
                new DataLockFailure
                {
                    ApprenticeshipPriceEpisodeIds = new List<long>(),
                    DataLockError = DataLockErrorCode.DLOCK_03
                }
            };

            aim = new LearningAim();

            mocker.Mock<ICalculatePeriodStartAndEndDate>()
                .Setup(x => x.GetPeriodDate(1, AcademicYear))
                .Returns(() => (new DateTime(2018, 8, 1), new DateTime(2018, 8, 31)));

            mocker.Mock<ICalculatePeriodStartAndEndDate>()
                .Setup(x => x.GetPeriodDate(2, AcademicYear))
                .Returns(() => (new DateTime(2018, 9, 1), new DateTime(2018, 9, 30)));
        }

        [Test]
        public void MatchedPriceEpisodeIsReturnedInValidPeriods()
        {
            apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 1,
                    AccountId = 21,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 90},
                        new ApprenticeshipPriceEpisodeModel{Id = 91},
                        new ApprenticeshipPriceEpisodeModel{Id = 92},
                        new ApprenticeshipPriceEpisodeModel{Id = 93},
                    },
                    EstimatedStartDate =new DateTime(2018, 8,1),
                    Status = ApprenticeshipStatus.Active
                },
                new ApprenticeshipModel
                {
                    Id = 2,
                    AccountId = 22,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 94},
                        new ApprenticeshipPriceEpisodeModel{Id = 95},
                        new ApprenticeshipPriceEpisodeModel{Id = 96},
                        new ApprenticeshipPriceEpisodeModel{Id = 97},
                    },
                    EstimatedStartDate =new DateTime(2018, 9,1),
                    Status = ApprenticeshipStatus.Active
                }
            };

            var earning = new OnProgrammeEarning
            {
                Periods = new List<EarningPeriod> { new EarningPeriod
                {
                    Amount = 1,
                    PriceEpisodeIdentifier = "pe-1" ,
                    Period = 1
                }
                }.AsReadOnly()
            };
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)))
                .Returns(() => new CourseValidationResult { MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 90 } });
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 2)))
                .Returns(() => new CourseValidationResult { DataLockFailures = dataLockFailures });

            var periods = mocker.Create<EarningPeriodsValidationProcessor>().ValidatePeriods(
                ukprn,
                1,
                priceEpisodes,
                earning.Periods.ToList(),
                (TransactionType)earning.Type,
                apprenticeships,
                aim,
                AcademicYear);

            periods.ValidPeriods.Count.Should().Be(1);
            periods.ValidPeriods.All(p => p.ApprenticeshipPriceEpisodeId == 90).Should().Be(true);
        }

        [Test]
        public void MatchesApprenticeship()
        {

            apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 1,
                    AccountId = 21,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 90},
                        new ApprenticeshipPriceEpisodeModel{Id = 91},
                        new ApprenticeshipPriceEpisodeModel{Id = 92},
                        new ApprenticeshipPriceEpisodeModel{Id = 93},
                    },
                    EstimatedStartDate =new DateTime(2018, 8,1),
                    Status = ApprenticeshipStatus.Active
                },
                new ApprenticeshipModel
                {
                    Id = 2,
                    AccountId = 22,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 94},
                        new ApprenticeshipPriceEpisodeModel{Id = 95},
                        new ApprenticeshipPriceEpisodeModel{Id = 96},
                        new ApprenticeshipPriceEpisodeModel{Id = 97},
                    },
                    EstimatedStartDate =new DateTime(2018, 9,1),
                    Status = ApprenticeshipStatus.Active
                }
            };

            var earning = new OnProgrammeEarning
            {
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
                        Amount = 1,
                        PriceEpisodeIdentifier = "pe-2",
                        Period = 2
                    },

              }.AsReadOnly()
            };
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)))
                .Returns(() => new CourseValidationResult { MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 90 } });
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 2)))
                .Returns(() => new CourseValidationResult { MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 96 } });

            var periods = mocker.Create<EarningPeriodsValidationProcessor>().ValidatePeriods(
                ukprn,
                1,
                priceEpisodes,
                earning.Periods.ToList(),
                (TransactionType)earning.Type,
                apprenticeships,
                aim,
                AcademicYear);
            periods.ValidPeriods.Count.Should().Be(2);
            periods.ValidPeriods.Any(p => p.ApprenticeshipPriceEpisodeId == 90 && p.AccountId == 21).Should().Be(true);
            periods.ValidPeriods.Any(p => p.ApprenticeshipPriceEpisodeId == 96 && p.AccountId == 22).Should().Be(true);
        }

        [Test]
        public void DataLocksAreReturnedInInvalidPeriods()
        {
            apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 1,
                    AccountId = 21,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 90},
                        new ApprenticeshipPriceEpisodeModel{Id = 91},
                        new ApprenticeshipPriceEpisodeModel{Id = 92},
                        new ApprenticeshipPriceEpisodeModel{Id = 93},
                    },
                    EstimatedStartDate =new DateTime(2018, 8,1),
                    Status = ApprenticeshipStatus.Active
                },
                new ApprenticeshipModel
                {
                    Id = 2,
                    AccountId = 22,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 94},
                        new ApprenticeshipPriceEpisodeModel{Id = 95},
                        new ApprenticeshipPriceEpisodeModel{Id = 96},
                        new ApprenticeshipPriceEpisodeModel{Id = 97},
                    },
                    EstimatedStartDate =new DateTime(2018, 9,1),
                    Status = ApprenticeshipStatus.Active
                }
            };


            var earning = new OnProgrammeEarning
            {
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
                        Amount = 1,
                        PriceEpisodeIdentifier = "pe-2",
                        Period = 2
                    },

                }.AsReadOnly()
            };

            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)))
                .Returns(() => new CourseValidationResult { MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 90 } });
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 2)))
                .Returns(() => new CourseValidationResult { DataLockFailures = dataLockFailures });

            var periods = mocker.Create<EarningPeriodsValidationProcessor>().ValidatePeriods(
                ukprn,
                1,
                priceEpisodes,
                earning.Periods.ToList(),
                (TransactionType)earning.Type,
                apprenticeships,
                aim,
                AcademicYear);

            periods.InValidPeriods.Count.Should().Be(1);
            periods.InValidPeriods[0].Period.Should().Be(2);
            periods.InValidPeriods.All(p => p.DataLockFailures[0].DataLockError == DataLockErrorCode.DLOCK_03).Should().BeTrue();
        }

        [Test]
        public void ZeroAmountPeriodsAreAddedToValidPeriod()
        {
            var earning = new OnProgrammeEarning
            {
                Periods = new List<EarningPeriod> { new EarningPeriod
                {
                    Amount = 0, PriceEpisodeIdentifier = "pe-1", Period = 1
                } }.AsReadOnly()
            };
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)))
                .Returns(() => new CourseValidationResult { MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 90 } });
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 2)))
                .Returns(() => new CourseValidationResult { DataLockFailures = dataLockFailures });

            var periods = mocker.Create<EarningPeriodsValidationProcessor>().ValidatePeriods(
                ukprn,
                1,
                priceEpisodes,
                earning.Periods.ToList(),
                (TransactionType)earning.Type,
                apprenticeships,
                aim,
                AcademicYear);

            periods.ValidPeriods.Count.Should().Be(1);
        }

        [Test]
        public void PopulatesTransferSenderAccountIdForMatchedApprenticeship()
        {
            apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 1,
                    AccountId = 21,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 90},
                        new ApprenticeshipPriceEpisodeModel{Id = 91},
                        new ApprenticeshipPriceEpisodeModel{Id = 92},
                        new ApprenticeshipPriceEpisodeModel{Id = 93},
                    },
                    EstimatedStartDate =new DateTime(2018, 8,1),
                    Status = ApprenticeshipStatus.Active,
                    TransferSendingEmployerAccountId = 999
                },
                new ApprenticeshipModel
                {
                    Id = 2,
                    AccountId = 22,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 94},
                        new ApprenticeshipPriceEpisodeModel{Id = 95},
                        new ApprenticeshipPriceEpisodeModel{Id = 96},
                        new ApprenticeshipPriceEpisodeModel{Id = 97},
                    },
                    EstimatedStartDate =new DateTime(2018, 9,1),
                    Status = ApprenticeshipStatus.Active
                }
            };

            var earning = new OnProgrammeEarning
            {
                Periods = new List<EarningPeriod> { new EarningPeriod
                {
                    Amount = 1,
                    PriceEpisodeIdentifier = "pe-1",
                    Period = 1
                } }.AsReadOnly()
            };
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)))
                .Returns(() => new CourseValidationResult { MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 90 } });
            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 2)))
                .Returns(() => new CourseValidationResult { MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 96 } });

            var periods = mocker.Create<EarningPeriodsValidationProcessor>().ValidatePeriods(
                ukprn,
                1, priceEpisodes,
                earning.Periods.ToList(),
                (TransactionType)earning.Type,
                apprenticeships,
                aim,
                AcademicYear);

            periods.ValidPeriods.Count.Should().Be(1);
            periods.ValidPeriods.Any(p => p.ApprenticeshipId == 1 && p.TransferSenderAccountId == 999).Should().Be(true);
        }

        [Test]
        public void OnlyValidateMostRecentApprenticeshipsWithinADeliveryPeriods()
        {

            aim.StandardCode = 403;

            apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 1,
                    AccountId = 21,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 90},
                        new ApprenticeshipPriceEpisodeModel{Id = 91},
                        new ApprenticeshipPriceEpisodeModel{Id = 92},
                        new ApprenticeshipPriceEpisodeModel{Id = 93},
                    },
                    EstimatedStartDate =new DateTime(2018, 9,15),
                    StopDate = new DateTime(2018, 9,17),
                    Status = ApprenticeshipStatus.Stopped,
                },
                new ApprenticeshipModel
                {
                    Id = 2,
                    AccountId = 21,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 94},
                        new ApprenticeshipPriceEpisodeModel{Id = 95},
                        new ApprenticeshipPriceEpisodeModel{Id = 96},
                        new ApprenticeshipPriceEpisodeModel{Id = 97},
                    },
                    EstimatedStartDate =new DateTime(2018, 9,15),
                    StopDate = new DateTime(2018, 9,15),
                    Status = ApprenticeshipStatus.Stopped,
                    StandardCode = aim.StandardCode
                },
                new ApprenticeshipModel
                {
                    Id = 3,
                    AccountId = 21,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 98},
                        new ApprenticeshipPriceEpisodeModel{Id = 99},
                        new ApprenticeshipPriceEpisodeModel{Id = 100},
                        new ApprenticeshipPriceEpisodeModel{Id = 101},
                    },
                    EstimatedStartDate =new DateTime(2018, 9,15),
                    StandardCode = 404,
                    Status = ApprenticeshipStatus.Active
                }
            };

            var earning = new OnProgrammeEarning
            {
                Periods = new List<EarningPeriod>
                {
                    new EarningPeriod
                    {
                        Amount = 1,
                        PriceEpisodeIdentifier = "pe-1",
                        Period = 2
                    }
                }.AsReadOnly(),

            };

            var earningPeriodsValidationProcessor = new EarningPeriodsValidationProcessor(
                new CourseValidationProcessor(
                    new StartDateValidator(),
                    new CompletionStoppedValidator(),
                    new OnProgrammeAndIncentiveStoppedValidator(new CalculatePeriodStartAndEndDate()),
                    new List<ICourseValidator> { new StandardCodeValidator() }
                ),
                new FunctionalSkillValidationProcessor(new List<ICourseValidator>()),
                new CalculatePeriodStartAndEndDate());


            var periods = earningPeriodsValidationProcessor
                .ValidatePeriods(ukprn, 1, priceEpisodes, earning.Periods.ToList(), (TransactionType)earning.Type, apprenticeships, aim, AcademicYear);

            mocker.Mock<ICourseValidationProcessor>()
                .Verify(x => x.ValidateCourse(It.Is<DataLockValidationModel>(o => o.Apprenticeship.Id == apprenticeships.Last().Id)),
                    Times.Once);

            periods.ValidPeriods.Count.Should().Be(0);
            periods.InValidPeriods.Count.Should().Be(1);
            periods.InValidPeriods.Any(p => p.DataLockFailures.Count() == 1 && p.DataLockFailures.All(d => d.DataLockError == DataLockErrorCode.DLOCK_04))
                .Should().Be(true);
        }

        [Test]
        public void OnlyValidateMostRecentActiveApprenticeshipsWithinADeliveryPeriod()
        {
            AcademicYear = 1920;
            aim.StandardCode = 403;
            apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 1,
                    AccountId = 21,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 1,
                            ApprenticeshipId = 1,
                            StartDate = new DateTime(2017, 6,1),
                        },
                    },
                    EstimatedStartDate = new DateTime(2017, 6,1),
                    EstimatedEndDate = new DateTime(2018, 7,1),
                    Status = ApprenticeshipStatus.Active,
                },
                new ApprenticeshipModel
                {
                    Id = 2,
                    AccountId = 21,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Id = 2,
                            ApprenticeshipId = 2,
                            StartDate = new DateTime(2018, 9,1),
                        },
                    },
                    EstimatedStartDate = new DateTime(2018, 9,1),
                    EstimatedEndDate = new DateTime(2020, 9,1),
                    Status = ApprenticeshipStatus.Active,
                }
            };

            var earning = new OnProgrammeEarning
            {
                Periods = new List<EarningPeriod>
                {
                    new EarningPeriod
                    {
                        Amount = 1,
                        PriceEpisodeIdentifier = "pe-1",
                        Period = 1
                    }
                }.AsReadOnly(),

            };

            mocker.Mock<ICalculatePeriodStartAndEndDate>()
                .Setup(x => x.GetPeriodDate(1, AcademicYear))
                .Returns(() => (new DateTime(2019, 8, 1), new DateTime(2019, 8, 31)));

            mocker.Mock<ICourseValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new CourseValidationResult
                {
                    DataLockFailures = new List<DataLockFailure>(),
                    MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel()
                });

            var periods = mocker.Create<EarningPeriodsValidationProcessor>()
                .ValidatePeriods(ukprn, 1, priceEpisodes, earning.Periods.ToList(), (TransactionType)earning.Type, apprenticeships, aim, AcademicYear);


            mocker.Mock<ICourseValidationProcessor>()
                .Verify(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)), Times.Never);

            mocker.Mock<ICourseValidationProcessor>()
                .Verify(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 2)), Times.Once);

        }

        [Test]
        public void ValidatesFunctionalSkillsWithLearningSupport()
        {
            apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 1,
                    AccountId = 21,
                    Ukprn = ukprn,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel{Id = 90},
                    },
                    EstimatedStartDate =new DateTime(2018, 8,1),
                    Status = ApprenticeshipStatus.Active,
                    TransferSendingEmployerAccountId = 999
                }
            };
            var earning = new FunctionalSkillEarning
            {
                Type = FunctionalSkillType.LearningSupport,
                Periods = new List<EarningPeriod>
                {
                    new EarningPeriod
                    {
                        Amount = 1,
                        Period = 1
                    }
                }.AsReadOnly()
            };
            mocker.Mock<IFunctionalSkillValidationProcessor>()
                .Setup(x => x.ValidateCourse(It.Is<DataLockValidationModel>(model => model.Apprenticeship.Id == 1)))
                .Returns(() => new CourseValidationResult
                {
                    MatchedPriceEpisode = new ApprenticeshipPriceEpisodeModel { Id = 90 },
                    DataLockFailures = new List<DataLockFailure>()
                });


            var periods = mocker.Create<EarningPeriodsValidationProcessor>().ValidateFunctionalSkillPeriods(
                ukprn, 1, earning.Periods.ToList(), (TransactionType)earning.Type, apprenticeships, aim, AcademicYear);

            periods.ValidPeriods.Should().HaveCount(1);
        }

        [Test]
        public void NoDLockForCompletedAndActiveApprenticeshipsInSameDeliveryPeriod()
        {
            AcademicYear = 1920;

            aim = new LearningAim
            {
                FrameworkCode = 490,
                PathwayCode = 1,
                ProgrammeType = 3,
                StandardCode = 0
            };

            apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 1,
                    Uln = 1,
                    AccountId = 21,
                    Ukprn = ukprn,
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
                            EndDate =new DateTime(2019, 8, 1),
                        }
                    },
                },
                new ApprenticeshipModel
                {
                    Id = 2,
                    Uln = 1,
                    AccountId = 21,
                    Ukprn = ukprn,
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

            priceEpisodes = new List<PriceEpisode>
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
           };

            var earning = new OnProgrammeEarning
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
            };
            
            var earningPeriodsValidationProcessor = new EarningPeriodsValidationProcessor(
                new CourseValidationProcessor(
                    new StartDateValidator(),
                    new CompletionStoppedValidator(),
                    new OnProgrammeAndIncentiveStoppedValidator(new CalculatePeriodStartAndEndDate()),
                    new List<ICourseValidator> { new StandardCodeValidator() }
                ),
                new FunctionalSkillValidationProcessor(new List<ICourseValidator>()),
                new CalculatePeriodStartAndEndDate());

            var periods = earningPeriodsValidationProcessor
                .ValidatePeriods(
                ukprn,
                1,
                priceEpisodes,
                earning.Periods.ToList(),
                (TransactionType)earning.Type,
                apprenticeships,
                aim, 
                AcademicYear);

            periods.ValidPeriods.Count.Should().Be(2);
           
        }

        [Test]
        public void OnlyValidateCommitmentsThatStartedBeforePriceEpisodes()
        {
            AcademicYear = 1920;
            aim = new LearningAim
            {
                FrameworkCode = 490,
                PathwayCode = 1,
                ProgrammeType = 3,
                StandardCode = 0
            };
            apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    Id = 1,
                    Uln = 1,
                    AccountId = 21,
                    Ukprn = ukprn,
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
                    Ukprn = ukprn,
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
            priceEpisodes = new List<PriceEpisode>
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
           };

            var earning = new OnProgrammeEarning
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
            };

            var earningPeriodsValidationProcessor = new EarningPeriodsValidationProcessor(
                new CourseValidationProcessor(
                    new StartDateValidator(),
                    new CompletionStoppedValidator(),
                    new OnProgrammeAndIncentiveStoppedValidator(new CalculatePeriodStartAndEndDate()),
                    new List<ICourseValidator> { new StandardCodeValidator() }
                ),
                new FunctionalSkillValidationProcessor(new List<ICourseValidator>()),
                new CalculatePeriodStartAndEndDate());

            var periods = earningPeriodsValidationProcessor
                .ValidatePeriods(
                ukprn,
                1,
                priceEpisodes,
                earning.Periods.ToList(),
                (TransactionType)earning.Type,
                apprenticeships,
                aim,
                AcademicYear);

            periods.ValidPeriods.Count.Should().Be(2);
            periods.ValidPeriods.All(x => x.ApprenticeshipId == 1).Should().BeTrue();

        }

    }
}