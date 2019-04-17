using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Mapping;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    [TestFixture]
    public class DataLockProcessorTests
    {
        private IMapper mapper;
        private ApprenticeshipContractType1EarningEvent earningEvent;
        private Mock<ILearnerMatcher> learnerMatcherMock;
        private Mock<ICourseValidatorsProcessor> courseValidationMock;
        private List<ApprenticeshipModel> apprenticeships;
        private const long Uln = 123;

        [OneTimeSetUp]
        public void Initialise()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DataLocksProfile>();
            });
            configuration.AssertConfigurationIsValid();
            mapper = configuration.CreateMapper();

            apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel{AccountId = 456, Uln = Uln},
                new ApprenticeshipModel{ AccountId = 456, Uln = Uln}
            };
        }

        [TearDown]
        public void CleanUp()
        {
            learnerMatcherMock.Verify();
            courseValidationMock.Verify();
        }

        [SetUp]
        public void Setup()
        {
            earningEvent = CreateTestEarningEvent(1,100m);
            learnerMatcherMock = new Mock<ILearnerMatcher>(MockBehavior.Strict);
            courseValidationMock = new Mock<ICourseValidatorsProcessor>(MockBehavior.Strict);
        }

        [Test]
        public async Task GivenNoDataLockErrorReturnedAllEarningPeriodsInPayableEvent()
        {
            learnerMatcherMock
                .Setup(x => x.MatchLearner(apprenticeships[0].Uln))
                .ReturnsAsync(() =>new LearnerMatchResult
                {
                    DataLockErrorCode = null, Apprenticeships = apprenticeships
                }).Verifiable();

            courseValidationMock
                .Setup(x => x.ValidateCourse(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new List<ValidationResult>())
                .Verifiable();

            var dataLockProcessor = new DataLockProcessor(mapper, learnerMatcherMock.Object, courseValidationMock.Object);
            var actual = await dataLockProcessor.GetPaymentEvent(earningEvent, default(CancellationToken));

            var payableEarning = actual as PayableEarningEvent;
            payableEarning.Should().NotBeNull();
            payableEarning.AccountId.Should().Be(456);
            payableEarning.OnProgrammeEarnings.Count.Should().Be(1);
            payableEarning.OnProgrammeEarnings.First().Periods.Count.Should().Be(1);
        }

        [Test]
        public async Task LearnerDataLockShouldReturnNonPayableEarningEvent()
        {
            learnerMatcherMock
                .Setup(x => x.MatchLearner(apprenticeships[0].Uln))
                .ReturnsAsync(() =>new LearnerMatchResult
                {
                    DataLockErrorCode = DataLockErrorCode.DLOCK_01, Apprenticeships = new List<ApprenticeshipModel>(apprenticeships)
                }).Verifiable();

            var dataLockProcessor = new DataLockProcessor(mapper, learnerMatcherMock.Object, courseValidationMock.Object);
            var actual = await dataLockProcessor.GetPaymentEvent(earningEvent, default(CancellationToken));

            var nonPayableEarningEvent = actual as NonPayableEarningEvent;
            nonPayableEarningEvent.Should().NotBeNull();
            nonPayableEarningEvent.Errors.Should().HaveCount(1);
            nonPayableEarningEvent.Errors.Should().Contain(DataLockErrorCode.DLOCK_01);
        }

        [Test]
        public async Task GivenCourseValidationDataLockIsReturnedMapOnlyValidEarningPeriods()
        {
            learnerMatcherMock
                .Setup(x => x.MatchLearner(apprenticeships[0].Uln))
                .ReturnsAsync(() => new LearnerMatchResult
                {
                    DataLockErrorCode = null,
                    Apprenticeships = new List<ApprenticeshipModel>(apprenticeships)

                }).Verifiable();

            var testEarningEvent = CreateTestEarningEvent(2, 100m);

            courseValidationMock
                .SetupSequence(x => x.ValidateCourse(It.IsAny<DataLockValidationModel>()))
                .Returns(() => new List<ValidationResult>
                {
                    new ValidationResult
                    {
                        DataLockErrorCode = DataLockErrorCode.DLOCK_09,
                        ApprenticeshipId = 1,
                        ApprenticeshipPriceEpisodeIdentifier = 100,
                        Period = 1
                    }
                })
                .Returns(() => new List<ValidationResult>());
            
            var dataLockProcessor = new DataLockProcessor(mapper, learnerMatcherMock.Object, courseValidationMock.Object);
            var actual = await dataLockProcessor.GetPaymentEvent(testEarningEvent, default(CancellationToken))
                .ConfigureAwait(false);

            courseValidationMock.Verify(x => x.ValidateCourse(It.IsAny<DataLockValidationModel>()), Times.Exactly(2));

            var payableEarning = actual as PayableEarningEvent;
            payableEarning.Should().NotBeNull();
            payableEarning.OnProgrammeEarnings.Count.Should().Be(1);

            var onProgrammeEarning = payableEarning.OnProgrammeEarnings.First();
            onProgrammeEarning.Periods.Count.Should().Be(1);

            var earningPeriod = onProgrammeEarning.Periods.Single();
            earningPeriod.Period.Should().Be(2);
        }

        [Test]
        public async Task GivenEarningAmountIsZeroThenAllPaymentWillBePayable()
        {
            learnerMatcherMock
                .Setup(x => x.MatchLearner(apprenticeships[0].Uln))
                .ReturnsAsync(() => new LearnerMatchResult
                {
                    DataLockErrorCode = null,
                    Apprenticeships = new List<ApprenticeshipModel>(apprenticeships)

                }).Verifiable();

            var testEarningEvent = CreateTestEarningEvent(2, decimal.Zero);
            
            var dataLockProcessor = new DataLockProcessor(mapper, learnerMatcherMock.Object, courseValidationMock.Object);
            var actual = await dataLockProcessor.GetPaymentEvent(testEarningEvent, default(CancellationToken)).ConfigureAwait(false);

            var payableEarning = actual as PayableEarningEvent;
            payableEarning.Should().NotBeNull();
            payableEarning.OnProgrammeEarnings.Count.Should().Be(1);

            var onProgrammeEarning = payableEarning.OnProgrammeEarnings.First();
            onProgrammeEarning.Periods.Count.Should().Be(2);
        }


        private ApprenticeshipContractType1EarningEvent CreateTestEarningEvent(byte periodsToCreate, decimal earningPeriodAmount)
        {
            var testEarningEvent = new ApprenticeshipContractType1EarningEvent
            {
                Learner = new Learner
                {
                    Uln = Uln,
                }
            };

            testEarningEvent.PriceEpisodes = new List<PriceEpisode>();

            var earningPeriods = new List<EarningPeriod>();
            for (byte i = 1; i <= periodsToCreate; i++)
            {
                testEarningEvent.PriceEpisodes.Add(new PriceEpisode
                {
                    StartDate = DateTime.UtcNow.AddDays(1),
                    Identifier = $"pe-{i}"
                });

                earningPeriods.Add(new EarningPeriod
                {
                    Amount = earningPeriodAmount,
                    Period = i,
                    PriceEpisodeIdentifier = $"pe-{i}"
                });
            }

            testEarningEvent.OnProgrammeEarnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {
                    Periods = new ReadOnlyCollection<EarningPeriod>(earningPeriods)
                }
            };

            return testEarningEvent;
        }

    }

}
