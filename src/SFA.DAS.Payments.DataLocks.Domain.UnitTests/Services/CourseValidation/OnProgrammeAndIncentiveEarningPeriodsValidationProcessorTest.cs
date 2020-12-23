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
            mocker.Provide<IStartDateValidator>(new StartDateValidator(false));
            mocker.Provide<ICalculatePeriodStartAndEndDate, CalculatePeriodStartAndEndDate>();
            mocker.Provide<IOnProgrammeAndIncentiveStoppedValidator, OnProgrammeAndIncentiveStoppedValidator>();
            mocker.Provide<ICompletionStoppedValidator, CompletionStoppedValidator>();
            mocker.Provide<ICourseValidationProcessor>(new CourseValidationProcessor(new List<ICourseValidator>
            {
                new StandardCodeValidator(),
                new ProgrammeTypeValidator()
            }));
        }

        [Test]
        public void GivenNoDLockMatchedPriceEpisodeIsReturnedInValidPeriods()
        {
            var apprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();
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

            periods.ValidPeriods.Count.Should().Be(3);
            periods.ValidPeriods.All(p => p.ApprenticeshipPriceEpisodeId == apprenticeships[0].ApprenticeshipPriceEpisodes[0].Id)
                .Should().Be(true);
        }

        [Test]
        public void PopulatesTransferSenderAccountIdForMatchedApprenticeship()
        {
            var apprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();
            var earningProcessor = mocker.Create<OnProgrammeAndIncentiveEarningPeriodsValidationProcessor>();

            apprenticeships[0].TransferSendingEmployerAccountId = 99;

            var periods = earningProcessor.ValidatePeriods(
                earningEvent.Ukprn,
                earningEvent.Learner.Uln,
                earningEvent.PriceEpisodes,
                earningEvent.OnProgrammeEarnings[0].Periods.ToList(),
                (TransactionType)earningEvent.OnProgrammeEarnings[0].Type,
                apprenticeships,
                earningEvent.LearningAim,
                earningEvent.CollectionPeriod.AcademicYear);

            periods.ValidPeriods.Count.Should().Be(3);
            periods.ValidPeriods
                .All(p =>
                    p.ApprenticeshipPriceEpisodeId == apprenticeships[0].ApprenticeshipPriceEpisodes[0].Id &&
                    p.TransferSenderAccountId == 99)
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

            earningEvent.OnProgrammeEarnings[0].Type = OnProgrammeEarningType.Completion;
            apprenticeships[0].Status = ApprenticeshipStatus.Stopped;
            apprenticeships[0].EstimatedStartDate = earningEvent.PriceEpisodes[0].ActualEndDate.GetValueOrDefault().AddDays(3);
            apprenticeships[0].ApprenticeshipPriceEpisodes
                .ForEach(x => x.StartDate = apprenticeships[0].EstimatedStartDate);

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

            var earningPeriods = earningEvent.OnProgrammeEarnings[0].Periods.Take(1).ToList();
            earningEvent.OnProgrammeEarnings[0].Type = OnProgrammeEarningType.Completion;
            apprenticeships[0].Status = ApprenticeshipStatus.Stopped;
            apprenticeships[0].StopDate = earningEvent.PriceEpisodes[0].ActualEndDate.GetValueOrDefault().AddDays(-5);

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
            var allApprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();

            earningEvent.OnProgrammeEarnings[0].Type = OnProgrammeEarningType.Learning;
            var apprenticeships = new List<ApprenticeshipModel> { allApprenticeships.First() };
            var earningPeriods = earningEvent.OnProgrammeEarnings[0].Periods.Take(1).ToList();
            earningEvent.OnProgrammeEarnings[0].Type = OnProgrammeEarningType.Completion;
            apprenticeships[0].Status = ApprenticeshipStatus.Stopped;
            apprenticeships[0].StopDate = earningEvent.PriceEpisodes[0].ActualEndDate.GetValueOrDefault().AddDays(-5);

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
        public void GivenThereIsOnProgrammeCompletionStoppedDLock10NoOtherDLockShouldBeGenerated()
        {
            var allApprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();

            earningEvent.CollectionYear = 1920;
            earningEvent.OnProgrammeEarnings[0].Periods[0].Period = 1;
            earningEvent.OnProgrammeEarnings[0].Type = OnProgrammeEarningType.Learning;
            earningEvent.LearningAim.StandardCode = 100;

            allApprenticeships[0].StopDate = new DateTime(2019, 07, 20);
            allApprenticeships[0].Status = ApprenticeshipStatus.Stopped;
            allApprenticeships[0].StandardCode = 900;

            var earningPeriods = new List<EarningPeriod>
            {
                earningEvent.OnProgrammeEarnings[0].Periods[0]
            };

            var apprenticeships = new List<ApprenticeshipModel>
            {
                allApprenticeships[0]
            };

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

            periods.ValidPeriods.Should().BeEmpty();
            periods.InValidPeriods.All(p => p.DataLockFailures.All(x =>
                x.ApprenticeshipId == apprenticeships[0].Id &&
                x.DataLockError == DataLockErrorCode.DLOCK_10 &&
                x.ApprenticeshipPriceEpisodeIds.All(o => apprenticeships[0].ApprenticeshipPriceEpisodes.Select(a => a.Id).Contains(o)))
            ).Should().BeTrue();

        }

        [Test]
        public void GivenThereIsCompletionStoppedDLock10NoOtherDLockShouldBeGeneratedForLatestApprenticeship()
        {
            var earningEvent = CreateEarningEventTestData();

            earningEvent.OnProgrammeEarnings[0].Type = OnProgrammeEarningType.Completion;

            var allApprenticeships = CreateApprenticeships();
            
            allApprenticeships.ForEach(x =>
            {
                x.StandardCode = earningEvent.LearningAim.StandardCode;
                x.ProgrammeType = earningEvent.LearningAim.ProgrammeType;
                x.FrameworkCode = earningEvent.LearningAim.FrameworkCode;
                x.PathwayCode = earningEvent.LearningAim.PathwayCode;
            });

            allApprenticeships[0].Status = ApprenticeshipStatus.Stopped;
            allApprenticeships[0].StopDate = earningEvent.PriceEpisodes[0].ActualEndDate.GetValueOrDefault().AddDays(-5);

            allApprenticeships[0].EstimatedStartDate = earningEvent.PriceEpisodes[0].EffectiveTotalNegotiatedPriceStartDate;
            allApprenticeships[0].ApprenticeshipPriceEpisodes.ForEach(o => o.StartDate = allApprenticeships[0].EstimatedStartDate);
            
            allApprenticeships[1].EstimatedStartDate = earningEvent.PriceEpisodes[0].EffectiveTotalNegotiatedPriceStartDate.AddYears(1);
            allApprenticeships[1].ApprenticeshipPriceEpisodes.ForEach(o => o.StartDate = allApprenticeships[1].EstimatedStartDate);
            allApprenticeships[1].Status = ApprenticeshipStatus.Active;

            var earningPeriods = new List<EarningPeriod>
            {
                earningEvent.OnProgrammeEarnings[0].Periods[0]
            };

            var earningProcessor = mocker.Create<OnProgrammeAndIncentiveEarningPeriodsValidationProcessor>();
            var periods = earningProcessor.ValidatePeriods(
                earningEvent.Ukprn,
                earningEvent.Learner.Uln,
                earningEvent.PriceEpisodes,
                earningPeriods,
                (TransactionType)earningEvent.OnProgrammeEarnings[0].Type,
                allApprenticeships,
                earningEvent.LearningAim,
                earningEvent.CollectionPeriod.AcademicYear);

            periods.ValidPeriods.Should().BeEmpty();
            periods.InValidPeriods.All(p => p.DataLockFailures.Any(x =>
                x.ApprenticeshipId == allApprenticeships[0].Id &&
                x.DataLockError == DataLockErrorCode.DLOCK_10 &&
                x.ApprenticeshipPriceEpisodeIds.All(o => allApprenticeships[0].ApprenticeshipPriceEpisodes.Select(a => a.Id).Contains(o)))
            ).Should().BeTrue();
        }

        [Test]
        public void GivenThereIsOnProgrammeAndIncentiveStoppedDLock10NoOtherDLockShouldBeGeneratedForLatestApprenticeship()
        {
            var allApprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();

            earningEvent.CollectionYear = 1920;
            earningEvent.OnProgrammeEarnings[0].Periods[0].Period = 1;
            earningEvent.OnProgrammeEarnings[0].Type = OnProgrammeEarningType.Learning;
            earningEvent.LearningAim.StandardCode = 100;

            allApprenticeships.ForEach(x =>
            {
                x.ApprenticeshipPriceEpisodes.ForEach(o => o.StartDate = x.EstimatedStartDate);
                x.StandardCode = 0;
                x.ProgrammeType = 0;
            });

            allApprenticeships[0].EstimatedStartDate = earningEvent.PriceEpisodes[0].EffectiveTotalNegotiatedPriceStartDate;
            allApprenticeships[0].ApprenticeshipPriceEpisodes.ForEach(o => o.StartDate = allApprenticeships[0].EstimatedStartDate);
            allApprenticeships[0].StopDate = new DateTime(2019, 07, 20);
            allApprenticeships[0].Status = ApprenticeshipStatus.Stopped;
            
            allApprenticeships[1].EstimatedStartDate = earningEvent.PriceEpisodes[0].EffectiveTotalNegotiatedPriceStartDate.AddYears(1);
            allApprenticeships[1].ApprenticeshipPriceEpisodes.ForEach(o => o.StartDate = allApprenticeships[1].EstimatedStartDate);
            allApprenticeships[1].Status = ApprenticeshipStatus.Active;

            var earningPeriods = new List<EarningPeriod>
            {
                earningEvent.OnProgrammeEarnings[0].Periods[0]
            };

            var earningProcessor = mocker.Create<OnProgrammeAndIncentiveEarningPeriodsValidationProcessor>();
            var periods = earningProcessor.ValidatePeriods(
                earningEvent.Ukprn,
                earningEvent.Learner.Uln,
                earningEvent.PriceEpisodes,
                earningPeriods,
                (TransactionType)earningEvent.OnProgrammeEarnings[0].Type,
                allApprenticeships,
                earningEvent.LearningAim,
                earningEvent.CollectionPeriod.AcademicYear);

            periods.ValidPeriods.Should().BeEmpty();
            periods.InValidPeriods.All(p => p.DataLockFailures.Any(x =>
                x.ApprenticeshipId == allApprenticeships[0].Id &&
                x.DataLockError == DataLockErrorCode.DLOCK_10 &&
                x.ApprenticeshipPriceEpisodeIds.All(o => allApprenticeships[0].ApprenticeshipPriceEpisodes.Select(a => a.Id).Contains(o)))
            ).Should().BeTrue();

        }

        [Test]
        public void ZeroAmountPeriodsAreAddedToValidPeriod()
        {
            var allApprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();
            earningEvent.OnProgrammeEarnings[0].Periods.ToList().ForEach(x => x.Amount = 0m);

            var earningProcessor = mocker.Create<OnProgrammeAndIncentiveEarningPeriodsValidationProcessor>();
            var periods = earningProcessor.ValidatePeriods(
                earningEvent.Ukprn,
                earningEvent.Learner.Uln,
                earningEvent.PriceEpisodes,
                earningEvent.OnProgrammeEarnings[0].Periods.ToList(),
                (TransactionType)earningEvent.OnProgrammeEarnings[0].Type,
                allApprenticeships,
                earningEvent.LearningAim,
                earningEvent.CollectionPeriod.AcademicYear);

            periods.InValidPeriods.Should().BeEmpty();
            periods.ValidPeriods.Should().HaveCount(3);
            periods.ValidPeriods.All(x => x.Amount == 0).Should().BeTrue();
        }

        [Test]
        public void OnlyValidateCommitmentsThatStartedBeforePriceEpisodes()
        {
            var apprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();

            earningEvent.PriceEpisodes[0].EffectiveTotalNegotiatedPriceStartDate = new DateTime(2018, 8, 30);
            apprenticeships[0].EstimatedStartDate = earningEvent.PriceEpisodes[0].EffectiveTotalNegotiatedPriceStartDate.AddDays(-1);
            apprenticeships[0].ApprenticeshipPriceEpisodes[0].StartDate = apprenticeships[0].EstimatedStartDate;

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

            periods.ValidPeriods.Count.Should().Be(3);
            periods.ValidPeriods.All(x =>
                    x.ApprenticeshipId == apprenticeships[0].Id &&
                    x.ApprenticeshipPriceEpisodeId == apprenticeships[0].ApprenticeshipPriceEpisodes[0].Id)
                .Should().BeTrue();
        }

        [Test]
        public void OnlyReturnDataLockForLatestApprenticeship()
        {
            var apprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();
            earningEvent.PriceEpisodes[0].EffectiveTotalNegotiatedPriceStartDate = new DateTime(2018, 8, 30);

            var priceEpisodes = new List<PriceEpisode> {earningEvent.PriceEpisodes[0]};
            var earningPeriods = earningEvent.OnProgrammeEarnings[0].Periods.Take(1).ToList();
            apprenticeships.ForEach(x =>
                {
                    x.EstimatedStartDate = earningEvent.PriceEpisodes[0].EffectiveTotalNegotiatedPriceStartDate.AddDays(1);
                    x.ApprenticeshipPriceEpisodes.ForEach(o => o.StartDate = x.EstimatedStartDate);
                });

            var latestApprenticeship = apprenticeships[1];

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

            periods.ValidPeriods.Should().BeEmpty();
            periods.InValidPeriods.All(p => p.DataLockFailures.All(x =>
                x.ApprenticeshipId == latestApprenticeship.Id &&
                x.DataLockError == DataLockErrorCode.DLOCK_09 &&
                x.ApprenticeshipPriceEpisodeIds.All(o => latestApprenticeship.ApprenticeshipPriceEpisodes.Select(a => a.Id).Contains(o)))
            ).Should().BeTrue();
        }

        [Test]
        public void ReturnAllDataLockForLatestApprenticeship()
        {
            var apprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();
            earningEvent.PriceEpisodes[0].EffectiveTotalNegotiatedPriceStartDate = new DateTime(2018, 8, 30);

            var priceEpisodes = new List<PriceEpisode> { earningEvent.PriceEpisodes[0] };
            var earningPeriods = earningEvent.OnProgrammeEarnings[0].Periods.Take(1).ToList();
            earningEvent.LearningAim.StandardCode = 999;
            earningEvent.LearningAim.ProgrammeType = 999;

            apprenticeships.ForEach(x =>
            {
                x.EstimatedStartDate = earningEvent.PriceEpisodes[0].EffectiveTotalNegotiatedPriceStartDate.AddDays(-1);
                x.ApprenticeshipPriceEpisodes.ForEach(o => o.StartDate = x.EstimatedStartDate);
                x.StandardCode = 0;
                x.ProgrammeType = 0;
            });

            apprenticeships[0].EstimatedStartDate = earningEvent.PriceEpisodes[0].EffectiveTotalNegotiatedPriceStartDate.AddDays(-10);
            apprenticeships[0].ApprenticeshipPriceEpisodes.ForEach(o => o.StartDate = apprenticeships[0].EstimatedStartDate);
            apprenticeships[0].StandardCode = 0;
            apprenticeships[0].ProgrammeType = 0;

            apprenticeships[1].EstimatedStartDate = earningEvent.PriceEpisodes[0].EffectiveTotalNegotiatedPriceStartDate.AddDays(-1);
            apprenticeships[1].ApprenticeshipPriceEpisodes.ForEach(o => o.StartDate = apprenticeships[1].EstimatedStartDate);
            apprenticeships[1].StandardCode = 0;
            apprenticeships[1].ProgrammeType = 0;
            
            var latestApprenticeship = apprenticeships[1];

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

            periods.ValidPeriods.Should().BeEmpty();

            var allDLockFailures = periods.InValidPeriods.SelectMany(p => p.DataLockFailures).ToList();
            allDLockFailures.Should().NotBeNullOrEmpty();
            allDLockFailures.All(x =>
                x.ApprenticeshipId == latestApprenticeship.Id && 
                x.ApprenticeshipPriceEpisodeIds.All(o => latestApprenticeship.ApprenticeshipPriceEpisodes.Select(a => a.Id).Contains(o)))
                .Should().BeTrue();
            allDLockFailures.Any(x => x.DataLockError == DataLockErrorCode.DLOCK_03 ).Should().BeTrue();
            allDLockFailures.Any(x =>  x.DataLockError == DataLockErrorCode.DLOCK_05).Should().BeTrue();
        }
        
        private ApprenticeshipContractType1EarningEvent CreateEarningEventTestData()
        {
            return new ApprenticeshipContractType1EarningEvent
            {
                Ukprn = 1,
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
                    },
                    new PriceEpisode
                    {
                        Identifier = "pe-2",
                        TotalNegotiatedPrice1 = 5000m,
                        AgreedPrice = 5000m,
                        EffectiveTotalNegotiatedPriceStartDate = new DateTime(2019,9,3),
                        PlannedEndDate =  new DateTime(2020,8,30),
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
                            new EarningPeriod
                            {
                                Amount = 3,
                                PriceEpisodeIdentifier = "pe-2",
                                Period = 3
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
                            ApprenticeshipId = 1,
                            Cost = 2000m,
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
                            Id = 2,
                            ApprenticeshipId = 2,
                            Cost = 2000m,
                            StartDate = new DateTime(2019, 9, 1),
                            EndDate = new DateTime(2020, 10, 1)
                        }
                    },
                },
            };
        }

    }
}
