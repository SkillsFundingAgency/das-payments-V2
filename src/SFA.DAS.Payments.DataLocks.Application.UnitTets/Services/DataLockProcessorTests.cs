﻿using AutoMapper;
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

        [SetUp]
        public void Setup()
        {
            earningEvent = new ApprenticeshipContractType1EarningEvent
            {
                Learner = new Learner
                {
                    Uln = 123,
                },
                PriceEpisodes = new List<PriceEpisode> { new PriceEpisode { StartDate = DateTime.UtcNow, Identifier = "pe-1" } },
                OnProgrammeEarnings = new List<OnProgrammeEarning>
                {
                    new OnProgrammeEarning
                    {
                        Periods = new ReadOnlyCollection<EarningPeriod>(
                            new List<EarningPeriod>
                            {
                                new EarningPeriod{Period = 1, PriceEpisodeIdentifier = "pe-1"}
                            })
                    }
                }
            };
        }

        [Test]
        public async Task TheReturnedObjectIsOfTheCorrectType()
        {
            var dataCacheMock = Mock.Of<IActorDataCache<List<ApprenticeshipModel>>>();

            var commitments = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    AccountId = 456,
                }
            };

            var learnerMatcherMock = new Mock<ILearnerMatcher>();
            learnerMatcherMock.Setup(x => x.MatchLearner(It.IsAny<long>())).ReturnsAsync(() =>
                new LearnerMatchResult
                { DataLockErrorCode = null, Apprenticeships = commitments });

            var courseValidationMock = new Mock<IProcessCourseValidator>();
            courseValidationMock
                .Setup(x => x.ValidateCourse(It.IsAny<DataLockValidation>()))
                .Returns(() => new List<ValidationResult>());
               

            Mock.Get(dataCacheMock).Setup(x => x.TryGet(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<ApprenticeshipModel>>(true, commitments));



            var actual = await new DataLockProcessor(mapper, learnerMatcherMock.Object, courseValidationMock.Object).Validate(earningEvent, default(CancellationToken));

            actual.Count.Should().Be(1);
            var payableEarningEvent = actual.First();
            payableEarningEvent.Should().BeOfType<PayableEarningEvent>();
            (payableEarningEvent as PayableEarningEvent).AccountId.Should().Be(456);
            var payableEarning = payableEarningEvent as PayableEarningEvent;
            payableEarning.OnProgrammeEarnings.Count.Should().Be(1);
            payableEarning.OnProgrammeEarnings.First().Periods.Count.Should().Be(1);
        }

        [Test]
        public async Task ThenLearnerDataLockReturned()
        {
            var dataCacheMock = Mock.Of<IActorDataCache<List<ApprenticeshipModel>>>();

            var commitments = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    AccountId = 456,
                }
            };

            var learnerMatcherMock = new Mock<ILearnerMatcher>();
            learnerMatcherMock.Setup(x => x.MatchLearner(It.IsAny<long>())).ReturnsAsync(() =>
                new LearnerMatchResult
                { DataLockErrorCode = DataLockErrorCode.DLOCK_01, Apprenticeships = new List<ApprenticeshipModel>(commitments) });

            var courseValidationMock = Mock.Of<IProcessCourseValidator>();

            Mock.Get(dataCacheMock).Setup(x => x.TryGet(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<ApprenticeshipModel>>(true, commitments));


            var actual = await new DataLockProcessor(mapper, learnerMatcherMock.Object, courseValidationMock)
                .Validate(earningEvent, default(CancellationToken));
            actual.Count.Should().Be(1);
            var payableEarningEvent = actual.First();

            payableEarningEvent.Should().BeOfType<NonPayableEarningEvent>();
            (payableEarningEvent as NonPayableEarningEvent).Errors.Contains(DataLockErrorCode.DLOCK_01);
        }

        [Test]
        public async Task ThenCourseValidationDataLockReturned()
        {
            var dataCacheMock = Mock.Of<IActorDataCache<List<ApprenticeshipModel>>>();

            var commitments = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    AccountId = 456,
                }
            };

            var learnerMatcherMock = new Mock<ILearnerMatcher>();
            learnerMatcherMock
                .Setup(x => x.MatchLearner(It.IsAny<long>()))
                .ReturnsAsync(() => new LearnerMatchResult
                {
                    DataLockErrorCode = null,
                    Apprenticeships = new List<ApprenticeshipModel>(commitments)

                });

            var courseValidationMock = new Mock<IProcessCourseValidator>();

            var testEarningEvent = CreateTestEarningEvent(2);
            
            courseValidationMock
                .SetupSequence(x => x.ValidateCourse(It.IsAny<DataLockValidation>()))
                .Returns(() => new List<ValidationResult>
                {
                    new ValidationResult
                    {
                        DataLockErrorCode = DataLockErrorCode.DLOCK_09,
                        ApprenticeshipId = 1,
                        ApprenticeshipPriceEpisodeIdentifier = "pe-1",
                        Period = 1
                    }
                })
                .Returns(() => new List<ValidationResult>());

            Mock.Get(dataCacheMock).Setup(x => x.TryGet(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<ApprenticeshipModel>>(true, commitments));

            var dataLockProcessor = new DataLockProcessor(mapper, learnerMatcherMock.Object, courseValidationMock.Object);
            var actual = await dataLockProcessor.Validate(testEarningEvent, default(CancellationToken));

            actual.Count.Should().Be(1);
            var payableEarningEvent = actual.First();

            payableEarningEvent.Should().BeOfType<PayableEarningEvent>();
            var payableEarning = payableEarningEvent as PayableEarningEvent;
            payableEarning.OnProgrammeEarnings.Count.Should().Be(1);

            var onProgrammeEarning = payableEarning.OnProgrammeEarnings.First();
            onProgrammeEarning.Periods.Count.Should().Be(1);

            var earningPeriod = onProgrammeEarning.Periods.First();

            earningPeriod.Period.Should().Be(2);
        }

        private ApprenticeshipContractType1EarningEvent CreateTestEarningEvent(byte periodsToCreate)
        {
            var testEarningEvent = new ApprenticeshipContractType1EarningEvent
            {
                Learner = new Learner
                {
                    Uln = 123,
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
