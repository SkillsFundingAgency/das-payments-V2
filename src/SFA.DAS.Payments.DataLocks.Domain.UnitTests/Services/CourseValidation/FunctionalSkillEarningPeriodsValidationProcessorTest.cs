using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.UnitTests.Services.CourseValidation
{
    [TestFixture]
    public class FunctionalSkillEarningPeriodsValidationProcessorTest
    {
        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mocker.Provide<ICalculatePeriodStartAndEndDate, CalculatePeriodStartAndEndDate>();
            mocker.Provide<IFunctionalSkillValidationProcessor>(new FunctionalSkillValidationProcessor(new List<ICourseValidator>
            {
                new ApprenticeshipPauseValidator()
            }));
        }

        [Test]
        public void ValidatesFunctionalSkillsWithLearningSupport()
        {
            var apprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();
            earningEvent.Earnings[0].Type = FunctionalSkillType.LearningSupport;
            var earningPeriods = new List<EarningPeriod> { earningEvent.Earnings[0].Periods[0] };

            var earningProcessor = mocker.Create<FunctionalSkillEarningPeriodsValidationProcessor>();
            var periods = earningProcessor.ValidatePeriods(
                earningEvent.Ukprn,
                earningEvent.Learner.Uln,
                earningPeriods,
                (TransactionType)earningEvent.Earnings[0].Type,
                apprenticeships,
                earningEvent.LearningAim,
                earningEvent.CollectionPeriod.AcademicYear);

            periods.ValidPeriods.Count.Should().Be(1);
            periods.ValidPeriods
                .All(p => p.ApprenticeshipPriceEpisodeId == apprenticeships[0].ApprenticeshipPriceEpisodes[0].Id)
                .Should().Be(true);
        }

        [Test]
        public void ShouldReturnDataLockFailuresInInvalidPeriods()
        {
            var apprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();

            apprenticeships[0].Status = ApprenticeshipStatus.Paused;

            earningEvent.Earnings[0].Type = FunctionalSkillType.LearningSupport;
            var earningPeriods = new List<EarningPeriod> { earningEvent.Earnings[0].Periods[0] };


            var earningProcessor = mocker.Create<FunctionalSkillEarningPeriodsValidationProcessor>();
            var periods = earningProcessor.ValidatePeriods(
                earningEvent.Ukprn,
                earningEvent.Learner.Uln,
                earningPeriods,
                (TransactionType)earningEvent.Earnings[0].Type,
                apprenticeships,
                earningEvent.LearningAim,
                earningEvent.CollectionPeriod.AcademicYear);
            
            periods.InValidPeriods.Should().HaveCount(1);
            periods.InValidPeriods.All(p => p.DataLockFailures.All(x =>
                x.ApprenticeshipId == apprenticeships[0].Id &&
                x.DataLockError == DataLockErrorCode.DLOCK_12 &&
                x.ApprenticeshipPriceEpisodeIds.All(o => apprenticeships[0].ApprenticeshipPriceEpisodes.Select(a => a.Id).Contains(o)))
            ).Should().BeTrue();
            
        }

        [Test]
        public void ShouldReturnValidAndInvalidPeriods()
        {
            var apprenticeships = CreateApprenticeships();
            var earningEvent = CreateEarningEventTestData();

            apprenticeships[0].Status = ApprenticeshipStatus.Paused;
            earningEvent.Earnings[0].Type = FunctionalSkillType.LearningSupport;

            var earningProcessor = mocker.Create<FunctionalSkillEarningPeriodsValidationProcessor>();
            var periods = earningProcessor.ValidatePeriods(
                earningEvent.Ukprn,
                earningEvent.Learner.Uln,
                earningEvent.Earnings[0].Periods.ToList(),
                (TransactionType)earningEvent.Earnings[0].Type,
                apprenticeships,
                earningEvent.LearningAim,
                earningEvent.CollectionPeriod.AcademicYear);
            
            periods.InValidPeriods.Should().HaveCount(1);
            periods.InValidPeriods.All(p => p.DataLockFailures.All(x =>
                x.ApprenticeshipId == apprenticeships[0].Id &&
                x.DataLockError == DataLockErrorCode.DLOCK_12 &&
                x.ApprenticeshipPriceEpisodeIds.All(o => apprenticeships[0].ApprenticeshipPriceEpisodes.Select(a => a.Id).Contains(o)))
            ).Should().BeTrue();

            periods.ValidPeriods.Count.Should().Be(1);
            periods.ValidPeriods
                .All(p => p.ApprenticeshipPriceEpisodeId == apprenticeships[1].ApprenticeshipPriceEpisodes[0].Id)
                .Should().Be(true);
        }

        private Act1FunctionalSkillEarningsEvent CreateEarningEventTestData()
        {
            return new Act1FunctionalSkillEarningsEvent
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
                Earnings = (new List<FunctionalSkillEarning>
                {
                        new FunctionalSkillEarning
                        {
                            Type = FunctionalSkillType.LearningSupport,
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
              }).AsReadOnly()
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
