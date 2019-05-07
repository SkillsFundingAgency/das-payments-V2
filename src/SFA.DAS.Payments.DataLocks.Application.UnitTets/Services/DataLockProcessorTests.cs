﻿using AutoMapper;
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
                                                apprenticeships))
                .Returns(() => (new List<ValidOnProgrammePeriod>
                {
                    new ValidOnProgrammePeriod
                    {
                        Apprenticeship = apprenticeships.FirstOrDefault(),
                        Period = earningEvent.OnProgrammeEarnings.FirstOrDefault()?.Periods.FirstOrDefault()
                    }
                }, new List<InvalidOnProgrammePeriod>()))
                .Verifiable();

            var dataLockProcessor = new DataLockProcessor(mapper, learnerMatcherMock.Object, onProgValidationMock.Object);
            var actual = await dataLockProcessor.GetPaymentEvent(earningEvent, default(CancellationToken));

            var payableEarning = actual as PayableEarningEvent;
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

            onProgValidationMock
                 .Setup(x => x.ValidatePeriods(apprenticeships[0].Uln, 
                    It.IsAny<List<PriceEpisode>>(),
                    It.IsAny<OnProgrammeEarning>(),
                    It.IsAny<List<ApprenticeshipModel>>()))
                .Returns(() => (new List<ValidOnProgrammePeriod>
                {
                    new ValidOnProgrammePeriod
                    {
                        Apprenticeship = apprenticeships.FirstOrDefault(),
                        Period = testEarningEvent.OnProgrammeEarnings[0].Periods[0]
                    }
                }, new List<InvalidOnProgrammePeriod>
                {
                    new InvalidOnProgrammePeriod
                    {
                        DataLockErrors = new List<DataLockErrorCode>{DataLockErrorCode.DLOCK_09},
                        Period = testEarningEvent.OnProgrammeEarnings[0].Periods[1]
                    }
                }))
                .Verifiable();

            var dataLockProcessor = new DataLockProcessor(mapper, learnerMatcherMock.Object, onProgValidationMock.Object);
            var actual = await dataLockProcessor.GetPaymentEvent(testEarningEvent, default(CancellationToken))
                .ConfigureAwait(false);

            var payableEarning = actual as PayableEarningEvent;
            payableEarning.Should().NotBeNull();
            payableEarning.OnProgrammeEarnings.Count.Should().Be(1);

            var onProgrammeEarning = payableEarning.OnProgrammeEarnings.First();
            onProgrammeEarning.Periods.Count.Should().Be(1);

            var earningPeriod = onProgrammeEarning.Periods.Single();
            earningPeriod.Period.Should().Be(1);
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
