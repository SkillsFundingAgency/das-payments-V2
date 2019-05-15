using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Mapping;
using SFA.DAS.Payments.DataLocks.Application.Services;
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
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    [TestFixture]
    public class DataLockProcessorTests
    {
        private IMapper mapper;
        private ApprenticeshipContractType1EarningEvent earningEvent;
        private Mock<ILearnerMatcher> learnerMatcherMock;
        private Mock<IOnProgrammePeriodsValidationProcessor> onProgValidationMock;
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
        }

        [TearDown]
        public void CleanUp()
        {
            learnerMatcherMock.Verify();
            onProgValidationMock.Verify();
        }

        [SetUp]
        public void Setup()
        {
            apprenticeships = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel{ Id = 1, AccountId = 456, Uln = Uln},
                new ApprenticeshipModel{ Id = 2 , AccountId = 456, Uln = Uln}
            };

            earningEvent = CreateTestEarningEvent(1, 100m);
            learnerMatcherMock = new Mock<ILearnerMatcher>(MockBehavior.Strict);
            onProgValidationMock = new Mock<IOnProgrammePeriodsValidationProcessor>(MockBehavior.Strict);
        }

        [Test]
        public async Task GivenNoDataLockErrorAllEarningPeriodsShouldBePayableEvent()
        {
            learnerMatcherMock
                .Setup(x => x.MatchLearner(apprenticeships[0].Uln))
                .ReturnsAsync(() => new LearnerMatchResult
                {
                    DataLockErrorCode = null,
                    Apprenticeships = apprenticeships
                })
                .Verifiable();

            onProgValidationMock
                .Setup(x => x.ValidatePeriods(apprenticeships[0].Uln,
                                                earningEvent.PriceEpisodes,
                                                earningEvent.OnProgrammeEarnings[0],
                                                apprenticeships, null))
                .Returns(() => 
                    (new List<EarningPeriod> {earningEvent.OnProgrammeEarnings.FirstOrDefault()?.Periods.FirstOrDefault()}, 
                    new List<EarningPeriod>()
                    )).Verifiable();

            var dataLockProcessor = new DataLockProcessor(mapper, learnerMatcherMock.Object, onProgValidationMock.Object);
            var dataLockEvents = await dataLockProcessor.GetPaymentEvents(earningEvent, default(CancellationToken));

            dataLockEvents.Should().NotBeNull();
            dataLockEvents.Should().HaveCount(1);

            var payableEarning = dataLockEvents[0] as PayableEarningEvent;
            payableEarning.Should().NotBeNull();
            payableEarning.OnProgrammeEarnings.Count.Should().Be(1);
            payableEarning.OnProgrammeEarnings.First().Periods.Count.Should().Be(1);
        }

        [Test]
        public async Task LearnerDataLockShouldReturnNonPayableEarningEvent()
        {
            learnerMatcherMock
                .Setup(x => x.MatchLearner(apprenticeships[0].Uln))
                .ReturnsAsync(() => new LearnerMatchResult
                {
                    DataLockErrorCode = DataLockErrorCode.DLOCK_01,
                    Apprenticeships = new List<ApprenticeshipModel>(apprenticeships)
                }).Verifiable();

            var dataLockProcessor = new DataLockProcessor(mapper, learnerMatcherMock.Object, onProgValidationMock.Object);
            var dataLockEvents = await dataLockProcessor.GetPaymentEvents(earningEvent, default(CancellationToken));
            dataLockEvents.Should().NotBeNull();
            dataLockEvents.Should().HaveCount(1);
            var nonPayableEarningEvent = dataLockEvents[0] as EarningFailedDataLockMatching;
            nonPayableEarningEvent.Should().NotBeNull();
            nonPayableEarningEvent.OnProgrammeEarnings
                .SelectMany(x => x.Periods)
                .All(p => p.DataLockFailures.All(d => d.DataLockError == DataLockErrorCode.DLOCK_01))
                .Should().BeTrue();
        }

        [Test]
        public async Task GivenCourseValidationDataLockIsReturnedMapBothValidAndInvalidEarningPeriods()
        {
            learnerMatcherMock
                .Setup(x => x.MatchLearner(apprenticeships[0].Uln))
                .ReturnsAsync(() => new LearnerMatchResult
                {
                    DataLockErrorCode = null,
                    Apprenticeships = new List<ApprenticeshipModel>(apprenticeships)

                }).Verifiable();

            var testEarningEvent = CreateTestEarningEvent(2, 100m);

            var periodExpected = testEarningEvent.OnProgrammeEarnings[0].Periods[1];
            periodExpected.DataLockFailures = new List<DataLockFailure>
            {
                new DataLockFailure
                {
                    DataLockError = DataLockErrorCode.DLOCK_09,
                    ApprenticeshipPriceEpisodeIds = new List<long>()
                }
            };

            onProgValidationMock
                 .Setup(x => x.ValidatePeriods(apprenticeships[0].Uln, 
                    It.IsAny<List<PriceEpisode>>(),
                    It.IsAny<OnProgrammeEarning>(),
                    It.IsAny<List<ApprenticeshipModel>>(), null))
                .Returns(() => (new List<EarningPeriod>
                {
                   testEarningEvent.OnProgrammeEarnings[0].Periods[0]
                }, new List<EarningPeriod>
                {
                     periodExpected
                }))
                .Verifiable();

            var dataLockProcessor = new DataLockProcessor(mapper, learnerMatcherMock.Object, onProgValidationMock.Object);
            var actual = await dataLockProcessor.GetPaymentEvents(testEarningEvent, default(CancellationToken))
                .ConfigureAwait(false);

            var payableEarnings = actual.OfType<PayableEarningEvent>().ToList();

            payableEarnings.Should().HaveCount(1);
            var payableEarning = payableEarnings[0];
            payableEarning.OnProgrammeEarnings.Count.Should().Be(1);

            var onProgrammeEarning = payableEarning.OnProgrammeEarnings.First();
            onProgrammeEarning.Periods.Count.Should().Be(1);

            var earningPeriod = onProgrammeEarning.Periods.Single();
            earningPeriod.Period.Should().Be(1);


            var failedDatalockEarnings = actual.OfType<EarningFailedDataLockMatching>().ToList();
            failedDatalockEarnings.Should().HaveCount(1);

            var failedDatalockEarning = failedDatalockEarnings[0];
            failedDatalockEarning.OnProgrammeEarnings.Count.Should().Be(1);

            var invalidOnProgrammeEarning = failedDatalockEarning.OnProgrammeEarnings[0];
            invalidOnProgrammeEarning.Periods.Count.Should().Be(1);

            var invalidEarningPeriod = invalidOnProgrammeEarning.Periods.Single();
            invalidEarningPeriod.Period.Should().Be(2);
            invalidEarningPeriod.DataLockFailures.Should().NotBeNull();
            invalidEarningPeriod.DataLockFailures.Should().HaveCount(1);
            invalidEarningPeriod.DataLockFailures[0].DataLockError.Should().Be(DataLockErrorCode.DLOCK_09);
        }

        private ApprenticeshipContractType1EarningEvent CreateTestEarningEvent(byte periodsToCreate, decimal earningPeriodAmount)
        {
            var testEarningEvent = new ApprenticeshipContractType1EarningEvent
            {
                Learner = new Learner { Uln = Uln, },
                PriceEpisodes = new List<PriceEpisode>()
            };


            var earningPeriods = new List<EarningPeriod>();
            for (byte i = 1; i <= periodsToCreate; i++)
            {
                testEarningEvent.PriceEpisodes.Add(new PriceEpisode
                {
                    EffectiveTotalNegotiatedPriceStartDate = DateTime.UtcNow.AddDays(1),
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
