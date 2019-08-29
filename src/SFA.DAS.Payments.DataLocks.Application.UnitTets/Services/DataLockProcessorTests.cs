using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    [TestFixture]
    public class DataLockProcessorTests
    {
        private IMapper mapper;
        private ApprenticeshipContractType1EarningEvent earningEvent;
        private Mock<ILearnerMatcher> learnerMatcherMock;
        private Mock<IEarningPeriodsValidationProcessor> onProgValidationMock;
        private List<ApprenticeshipModel> apprenticeships;

        private  Mock<IGenerateApprenticeshipEarningCacheKey> generateApprenticeshipEarningCacheKey;
        private  Mock<IActorDataCache<ApprenticeshipContractType1EarningEvent>> apprenticeshipContractType1EarningEventCache;
        private  Mock<IActorDataCache<Act1FunctionalSkillEarningsEvent>> act1FunctionalSkillEarningsEventCache;
        private  Mock<IActorDataCache<PayableEarningEvent>> payableEarningEventCache;
        private  Mock<IActorDataCache<PayableFunctionalSkillEarningEvent>> payableFunctionalSkillEarningEventCache;

        private const long Uln = 123;
        private const int AcademicYear = 1819;
        private LearningAim aim;
        private const long Ukprn = 123;
        const string act1EarningsKey = "Act1EarningsKey";
        const string act1PayableEarningsKey = "Act1PayableEarningsKey";
        const string act1FunctionalSkillEarningsKey = "Act1FunctionalSkillEarningsKey";
        const string act1FunctionalSkillPayableEarningsKey = "Act1FunctionalSkillPayableEarningsKey";

        [OneTimeSetUp]
        public void Initialise()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DataLocksProfile>();
            });
            configuration.AssertConfigurationIsValid();
            mapper = configuration.CreateMapper();
            aim = new LearningAim();
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
                new ApprenticeshipModel{ Id = 1, AccountId = 456, Uln = Uln, Ukprn = Ukprn},
                new ApprenticeshipModel{ Id = 2 , AccountId = 456, Uln = Uln, Ukprn = Ukprn}
            };

            earningEvent = CreateTestEarningEvent(1, 100m, aim);
            earningEvent.LearningAim = aim;

            learnerMatcherMock = new Mock<ILearnerMatcher>(MockBehavior.Strict);
            onProgValidationMock = new Mock<IEarningPeriodsValidationProcessor>(MockBehavior.Strict);

            generateApprenticeshipEarningCacheKey = new Mock<IGenerateApprenticeshipEarningCacheKey>(MockBehavior.Strict);
            act1FunctionalSkillEarningsEventCache = new Mock<IActorDataCache<Act1FunctionalSkillEarningsEvent>>();
            apprenticeshipContractType1EarningEventCache = new Mock<IActorDataCache<ApprenticeshipContractType1EarningEvent>>();
            payableEarningEventCache = new Mock<IActorDataCache<PayableEarningEvent>>();
            payableFunctionalSkillEarningEventCache = new Mock<IActorDataCache<PayableFunctionalSkillEarningEvent>>();
            
            generateApprenticeshipEarningCacheKey
                .Setup(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1EarningsKey, Ukprn, Uln))
                .Returns(act1EarningsKey)
                .Verifiable();

            apprenticeshipContractType1EarningEventCache
                .Setup(x => x.AddOrReplace(act1EarningsKey,
                    It.IsAny<ApprenticeshipContractType1EarningEvent>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            generateApprenticeshipEarningCacheKey
                .Setup(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1PayableEarningsKey, Ukprn, Uln))
                .Returns(act1PayableEarningsKey)
                .Verifiable();

            payableEarningEventCache
                .Setup(x => x.AddOrReplace(act1PayableEarningsKey,
                    It.IsAny<PayableEarningEvent>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            generateApprenticeshipEarningCacheKey
                .Setup(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1FunctionalSkillEarningsKey, Ukprn, Uln))
                .Returns(act1FunctionalSkillEarningsKey)
                .Verifiable();

            act1FunctionalSkillEarningsEventCache
                .Setup(x => x.AddOrReplace(act1FunctionalSkillEarningsKey,
                    It.IsAny<Act1FunctionalSkillEarningsEvent>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            generateApprenticeshipEarningCacheKey
                .Setup(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1FunctionalSkillPayableEarningsKey, Ukprn, Uln))
                .Returns(act1FunctionalSkillPayableEarningsKey)
                .Verifiable();

            payableFunctionalSkillEarningEventCache
                .Setup(x => x.AddOrReplace(act1FunctionalSkillPayableEarningsKey,
                    It.IsAny<PayableFunctionalSkillEarningEvent>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            
        }

        [Test]
        public async Task GivenNoDataLockErrorAllEarningPeriodsShouldBePayableEvent()
        {
            learnerMatcherMock
                .Setup(x => x.MatchLearner(apprenticeships[0].Ukprn,apprenticeships[0].Uln))
                .ReturnsAsync(() => new LearnerMatchResult
                {
                    DataLockErrorCode = null,
                    Apprenticeships = apprenticeships
                })
                .Verifiable();

            onProgValidationMock
                .Setup(x => x.ValidatePeriods(Ukprn,apprenticeships[0].Uln,
                                                earningEvent.PriceEpisodes,
                                                It.IsAny<List<EarningPeriod>>(),
                                                (TransactionType)earningEvent.OnProgrammeEarnings[0].Type,
                                                apprenticeships,
                                                aim,
                                                AcademicYear))
                .Returns(() =>
                    (new List<EarningPeriod> { earningEvent.OnProgrammeEarnings.FirstOrDefault()?.Periods.FirstOrDefault() },
                    new List<EarningPeriod>()
                    )).Verifiable();



            var dataLockProcessor = new DataLockProcessor(mapper,
                learnerMatcherMock.Object,
                onProgValidationMock.Object,
                generateApprenticeshipEarningCacheKey.Object,
                act1FunctionalSkillEarningsEventCache.Object,
                apprenticeshipContractType1EarningEventCache.Object,
                payableEarningEventCache.Object,
                payableFunctionalSkillEarningEventCache.Object);


            var dataLockEvents = await dataLockProcessor.GetPaymentEvents(earningEvent, default(CancellationToken));

            dataLockEvents.Should().NotBeNull();
            dataLockEvents.Should().HaveCount(1);

            var payableEarning = dataLockEvents[0] as PayableEarningEvent;
            payableEarning.Should().NotBeNull();
            payableEarning.OnProgrammeEarnings.Count.Should().Be(1);
            payableEarning.OnProgrammeEarnings.First().Periods.Count.Should().Be(1);

            payableEarning.IncentiveEarnings.Should().NotBeNull();
            payableEarning.IncentiveEarnings.Count.Should().Be(1);
            payableEarning.IncentiveEarnings.First().Periods.Count.Should().Be(1);
            
            generateApprenticeshipEarningCacheKey
                .Verify(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1EarningsKey, Ukprn, Uln));
            
            apprenticeshipContractType1EarningEventCache
                .Verify(x => x.AddOrReplace(act1EarningsKey, earningEvent, It.IsAny<CancellationToken>()));
            
            generateApprenticeshipEarningCacheKey
                .Verify(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1PayableEarningsKey, Ukprn, Uln));
            
            payableEarningEventCache
                .Verify(x => x.AddOrReplace(act1PayableEarningsKey, payableEarning, It.IsAny<CancellationToken>()));
            
        }

        [Test]
        public async Task LearnerDataLockForEarningWithNoIncentivesShouldReturnValidNonPayableEarningEvent()
        {
            earningEvent.IncentiveEarnings = new List<IncentiveEarning>();

            learnerMatcherMock
                .Setup(x => x.MatchLearner(apprenticeships[0].Ukprn, apprenticeships[0].Uln))
                .ReturnsAsync(() => new LearnerMatchResult
                {
                    DataLockErrorCode = DataLockErrorCode.DLOCK_01,
                    Apprenticeships = new List<ApprenticeshipModel>(apprenticeships)
                }).Verifiable();

            var dataLockProcessor = new DataLockProcessor(mapper,
                learnerMatcherMock.Object,
                onProgValidationMock.Object,
                generateApprenticeshipEarningCacheKey.Object,
                act1FunctionalSkillEarningsEventCache.Object,
                apprenticeshipContractType1EarningEventCache.Object,
                payableEarningEventCache.Object,
                payableFunctionalSkillEarningEventCache.Object);
            
            var dataLockEvents = await dataLockProcessor.GetPaymentEvents(earningEvent, default(CancellationToken));
            dataLockEvents.Should().NotBeNull();
            dataLockEvents.Should().HaveCount(1);
            var nonPayableEarningEvent = dataLockEvents[0] as EarningFailedDataLockMatching;
            nonPayableEarningEvent.Should().NotBeNull();
            nonPayableEarningEvent.OnProgrammeEarnings
                .SelectMany(x => x.Periods)
                .All(p => p.DataLockFailures.All(d => d.DataLockError == DataLockErrorCode.DLOCK_01))
                .Should().BeTrue();


            generateApprenticeshipEarningCacheKey
                .Verify(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1EarningsKey, Ukprn, Uln), Times.Once);

            apprenticeshipContractType1EarningEventCache
                .Verify(x => x.AddOrReplace(act1EarningsKey, earningEvent, It.IsAny<CancellationToken>()), Times.Once);

            generateApprenticeshipEarningCacheKey
                .Verify(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1PayableEarningsKey, Ukprn, Uln), Times.Never);

            payableEarningEventCache
                .Verify(x => x.AddOrReplace(act1PayableEarningsKey, It.IsAny<PayableEarningEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task LearnerDataLockForEarningWithIncentivesShouldReturnValidNonPayableEarningEvent()
        {
            learnerMatcherMock
                .Setup(x => x.MatchLearner(apprenticeships[0].Ukprn, apprenticeships[0].Uln))
                .ReturnsAsync(() => new LearnerMatchResult
                {
                    DataLockErrorCode = DataLockErrorCode.DLOCK_01,
                    Apprenticeships = new List<ApprenticeshipModel>(apprenticeships)
                }).Verifiable();

            var dataLockProcessor = new DataLockProcessor(mapper,
                learnerMatcherMock.Object,
                onProgValidationMock.Object,
                generateApprenticeshipEarningCacheKey.Object,
                act1FunctionalSkillEarningsEventCache.Object,
                apprenticeshipContractType1EarningEventCache.Object,
                payableEarningEventCache.Object,
                payableFunctionalSkillEarningEventCache.Object);
            
            var dataLockEvents = await dataLockProcessor.GetPaymentEvents(earningEvent, default(CancellationToken));
            dataLockEvents.Should().NotBeNull();
            dataLockEvents.Should().HaveCount(1);
            var nonPayableEarningEvent = dataLockEvents[0] as EarningFailedDataLockMatching;
            nonPayableEarningEvent.Should().NotBeNull();
            nonPayableEarningEvent.IncentiveEarnings
                .SelectMany(x => x.Periods)
                .All(p => p.DataLockFailures.All(d => d.DataLockError == DataLockErrorCode.DLOCK_01))
                .Should().BeTrue();
            
            generateApprenticeshipEarningCacheKey
                .Verify(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1EarningsKey, Ukprn, Uln), Times.Once);

            apprenticeshipContractType1EarningEventCache
                .Verify(x => x.AddOrReplace(act1EarningsKey, earningEvent, It.IsAny<CancellationToken>()), Times.Once);

            generateApprenticeshipEarningCacheKey
                .Verify(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1PayableEarningsKey, Ukprn, Uln), Times.Never);

            payableEarningEventCache
                .Verify(x => x.AddOrReplace(act1PayableEarningsKey, It.IsAny<PayableEarningEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        
        [Test]
        public async Task GivenCourseValidationDataLockIsReturnedMapBothValidAndInvalidEarningPeriods()
        {
            learnerMatcherMock
                .Setup(x => x.MatchLearner(apprenticeships[0].Ukprn, apprenticeships[0].Uln))
                .ReturnsAsync(() => new LearnerMatchResult
                {
                    DataLockErrorCode = null,
                    Apprenticeships = new List<ApprenticeshipModel>(apprenticeships)

                }).Verifiable();

            var testEarningEvent = CreateTestEarningEvent(2, 100m, aim);
            testEarningEvent.IncentiveEarnings = new List<IncentiveEarning>();

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
                 .Setup(x => x.ValidatePeriods(Ukprn, apprenticeships[0].Uln,
                    It.IsAny<List<PriceEpisode>>(),
                    It.IsAny<List<EarningPeriod>>(),
                    It.IsAny<TransactionType>(),
                    It.IsAny<List<ApprenticeshipModel>>(),
                    aim,
                    AcademicYear))
                .Returns(() => (new List<EarningPeriod>
                {
                   testEarningEvent.OnProgrammeEarnings[0].Periods[0]
                }, new List<EarningPeriod>
                {
                     periodExpected
                }))
                .Verifiable();

            var dataLockProcessor = new DataLockProcessor(mapper,
                learnerMatcherMock.Object,
                onProgValidationMock.Object,
                generateApprenticeshipEarningCacheKey.Object,
                act1FunctionalSkillEarningsEventCache.Object,
                apprenticeshipContractType1EarningEventCache.Object,
                payableEarningEventCache.Object,
                payableFunctionalSkillEarningEventCache.Object);

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
            
            generateApprenticeshipEarningCacheKey
                .Verify(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1EarningsKey, Ukprn, Uln));

            apprenticeshipContractType1EarningEventCache
                .Verify(x => x.AddOrReplace(act1EarningsKey, testEarningEvent, It.IsAny<CancellationToken>()));

            generateApprenticeshipEarningCacheKey
                .Verify(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1PayableEarningsKey, Ukprn, Uln));

            payableEarningEventCache
                .Verify(x => x.AddOrReplace(act1PayableEarningsKey, payableEarning, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task CourseValidationDataLockForEarningWithIncentivesMapBothValidAndInvalidIncentivesAndOnprogEarningPeriods()
        {
            learnerMatcherMock
                .Setup(x => x.MatchLearner(apprenticeships[0].Ukprn, apprenticeships[0].Uln))
                .ReturnsAsync(() => new LearnerMatchResult
                {
                    DataLockErrorCode = null,
                    Apprenticeships = new List<ApprenticeshipModel>(apprenticeships)

                }).Verifiable();

            var testEarningEvent = CreateTestEarningEvent(2, 100m, aim);

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
                 .Setup(x => x.ValidatePeriods(Ukprn, apprenticeships[0].Uln,
                    It.IsAny<List<PriceEpisode>>(),
                    It.IsAny<List<EarningPeriod>>(),
                    It.IsAny<TransactionType>(),
                    It.IsAny<List<ApprenticeshipModel>>(),
                    aim,
                    AcademicYear))
                .Returns(() => (new List<EarningPeriod>
                {
                   testEarningEvent.OnProgrammeEarnings[0].Periods[0]
                }, new List<EarningPeriod>
                {
                     periodExpected
                }))
                .Verifiable();

            var dataLockProcessor = new DataLockProcessor(mapper,
                learnerMatcherMock.Object,
                onProgValidationMock.Object,
                generateApprenticeshipEarningCacheKey.Object,
                act1FunctionalSkillEarningsEventCache.Object,
                apprenticeshipContractType1EarningEventCache.Object,
                payableEarningEventCache.Object,
                payableFunctionalSkillEarningEventCache.Object);

            var actualDataLockEvents = await dataLockProcessor.GetPaymentEvents(testEarningEvent, default(CancellationToken))
                .ConfigureAwait(false);

            var payableEarnings = actualDataLockEvents.OfType<PayableEarningEvent>().ToList();

            payableEarnings.Should().HaveCount(1);
            var payableEarning = payableEarnings[0];

            //validate OnProgrammeEarnings
            payableEarning.OnProgrammeEarnings.Count.Should().Be(1);

            var onProgrammeEarning = payableEarning.OnProgrammeEarnings.First();
            onProgrammeEarning.Periods.Count.Should().Be(1);

            var earningPeriod = onProgrammeEarning.Periods.Single();
            earningPeriod.Period.Should().Be(1);

            
            var failedDatalockEarnings = actualDataLockEvents.OfType<EarningFailedDataLockMatching>().ToList();
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

            //Validate Incentives
            payableEarning.IncentiveEarnings.Count.Should().Be(1);
            var incentiveEarnings = payableEarning.IncentiveEarnings.First();
            incentiveEarnings.Periods.Count.Should().Be(1);

            var incentiveEarningPeriod = incentiveEarnings.Periods.Single();
            incentiveEarningPeriod.Period.Should().Be(1);
            
            failedDatalockEarning.IncentiveEarnings.Count.Should().Be(1);

            var invalidIncentiveEarning = failedDatalockEarning.IncentiveEarnings[0];
            invalidIncentiveEarning.Periods.Count.Should().Be(1);

            var invalidIncentiveEarningPeriod = invalidIncentiveEarning.Periods.Single();
            invalidIncentiveEarningPeriod.Period.Should().Be(2);
            invalidIncentiveEarningPeriod.DataLockFailures.Should().NotBeNull();
            invalidIncentiveEarningPeriod.DataLockFailures.Should().HaveCount(1);
            invalidIncentiveEarningPeriod.DataLockFailures[0].DataLockError.Should().Be(DataLockErrorCode.DLOCK_09);



            generateApprenticeshipEarningCacheKey
                .Verify(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1EarningsKey, Ukprn, Uln));

            apprenticeshipContractType1EarningEventCache
                .Verify(x => x.AddOrReplace(act1EarningsKey, testEarningEvent, It.IsAny<CancellationToken>()));

            generateApprenticeshipEarningCacheKey
                .Verify(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1PayableEarningsKey, Ukprn, Uln));

            payableEarningEventCache
                .Verify(x => x.AddOrReplace(act1PayableEarningsKey, payableEarning, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task CourseValidationForFunctionalSkillMapsValidAndInvalidPeriods()
        {
            learnerMatcherMock
                .Setup(x => x.MatchLearner(Ukprn, apprenticeships[0].Uln))
                .ReturnsAsync(() => new LearnerMatchResult
                {
                    DataLockErrorCode = null,
                    Apprenticeships = new List<ApprenticeshipModel>(apprenticeships)

                }).Verifiable();

            var testEarningEvent = CreateTestFunctionalSkillEarningEvent(2, 100m, aim);

            var periodExpected = testEarningEvent.Earnings[0].Periods[1];
            periodExpected.DataLockFailures = new List<DataLockFailure>
            {
                new DataLockFailure
                {
                    DataLockError = DataLockErrorCode.DLOCK_09,
                    ApprenticeshipPriceEpisodeIds = new List<long>()
                }
            };

            onProgValidationMock
                .Setup(x => x.ValidateFunctionalSkillPeriods(Ukprn, apprenticeships[0].Uln,
                    It.IsAny<List<PriceEpisode>>(),
                    It.IsAny<List<EarningPeriod>>(),
                    It.IsAny<TransactionType>(),
                    It.IsAny<List<ApprenticeshipModel>>(),
                    aim,
                    AcademicYear))
                .Returns(() => (new List<EarningPeriod>
                {
                    testEarningEvent.Earnings[0].Periods[0]
                }, new List<EarningPeriod>
                {
                    periodExpected
                }))
                .Verifiable();

            var dataLockProcessor = new DataLockProcessor(mapper,
                learnerMatcherMock.Object,
                onProgValidationMock.Object,
                generateApprenticeshipEarningCacheKey.Object,
                act1FunctionalSkillEarningsEventCache.Object,
                apprenticeshipContractType1EarningEventCache.Object,
                payableEarningEventCache.Object,
                payableFunctionalSkillEarningEventCache.Object);

            var actualDataLockEvents = await dataLockProcessor.GetFunctionalSkillPaymentEvents(testEarningEvent, default(CancellationToken))
                .ConfigureAwait(false);

            var payableEarnings = actualDataLockEvents.OfType<PayableFunctionalSkillEarningEvent>().ToList();
            payableEarnings.Should().HaveCount(1);

            var failedDataLockEarnings = actualDataLockEvents.OfType<FunctionalSkillEarningFailedDataLockMatching>().ToList();
            failedDataLockEarnings.Should().HaveCount(1);
            
            generateApprenticeshipEarningCacheKey
                .Verify(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1FunctionalSkillEarningsKey, Ukprn, Uln));
               
            act1FunctionalSkillEarningsEventCache
                .Verify(x => x.AddOrReplace(act1FunctionalSkillEarningsKey, testEarningEvent, It.IsAny<CancellationToken>()));
               
            generateApprenticeshipEarningCacheKey
                .Verify(x => x.GenerateKey(ApprenticeshipEarningCacheKeyTypes.Act1FunctionalSkillPayableEarningsKey, Ukprn, Uln));
               
            payableFunctionalSkillEarningEventCache
                .Verify(x => x.AddOrReplace(act1FunctionalSkillPayableEarningsKey, payableEarnings[0], It.IsAny<CancellationToken>()));
             
        }
        
        private ApprenticeshipContractType1EarningEvent CreateTestEarningEvent(byte periodsToCreate, decimal earningPeriodAmount, LearningAim testAim)
        {
            var testEarningEvent = new ApprenticeshipContractType1EarningEvent
            {
                Learner = new Learner { Uln = Uln, },
                PriceEpisodes = new List<PriceEpisode>(),
                CollectionYear = AcademicYear,
                Ukprn = Ukprn
            };

            testEarningEvent.OnProgrammeEarnings = new List<OnProgrammeEarning>
            {
                new OnProgrammeEarning
                {
                    Periods = new ReadOnlyCollection<EarningPeriod>(GenerateEarningPeriod(periodsToCreate, earningPeriodAmount, testEarningEvent) )
                }
            };

            testEarningEvent.IncentiveEarnings = new List<IncentiveEarning>
            {
                new IncentiveEarning
                {
                    Periods = new ReadOnlyCollection<EarningPeriod>(GenerateEarningPeriod(periodsToCreate, earningPeriodAmount, testEarningEvent))
                }
            };

            testEarningEvent.LearningAim = testAim;

            return testEarningEvent;
        }

        private Act1FunctionalSkillEarningsEvent CreateTestFunctionalSkillEarningEvent(byte periodsToCreate, decimal earningPeriodAmount, LearningAim testAim)
        {
            var testEarningEvent = new Act1FunctionalSkillEarningsEvent
            {
                Ukprn = Ukprn,
                Learner = new Learner { Uln = Uln, },
                PriceEpisodes = new List<PriceEpisode>(),
                CollectionYear = AcademicYear
            };

            var functionalSkillEarnings = new List<FunctionalSkillEarning>
            {
                new FunctionalSkillEarning
                {
                    Periods = new ReadOnlyCollection<EarningPeriod>(GenerateEarningPeriod(periodsToCreate, earningPeriodAmount, testEarningEvent) )
                }
            };

            testEarningEvent.Earnings = functionalSkillEarnings.AsReadOnly();

            testEarningEvent.LearningAim = testAim;

            return testEarningEvent;
        }

        private static List<EarningPeriod> GenerateEarningPeriod(byte periodsToCreate, decimal earningPeriodAmount, ApprenticeshipContractType1EarningEvent testEarningEvent)
        {
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

            return earningPeriods;
        }

        private static List<EarningPeriod> GenerateEarningPeriod(byte periodsToCreate, decimal earningPeriodAmount, Act1FunctionalSkillEarningsEvent testEarningEvent)
        {
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

            return earningPeriods;
        }
    }
}
