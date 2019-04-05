using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors;
using Moq;
using NUnit.Framework;
using ServiceFabric.Mocks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Mapping;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.DataLockService.UnitTests.GivenADataLockService
{
    [TestFixture]
    public class WhenCallingHandlePayment
    {
        private IMapper mapper;

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

        [Test]
        public async Task TheReturnedObjectIsOfTheCorrectType()
        {
            var actorService = MockActorServiceFactory.CreateActorServiceForActor<DataLockService>();
            var paymentLoggerMock = Mock.Of<IPaymentLogger>();
            var commitmentRepositoryMock = Mock.Of<IApprenticeshipRepository>();
            var dataCacheMock = Mock.Of<IActorDataCache<List<ApprenticeshipModel>>>();

            var commitments = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    AccountId = 456,
                }
            };

            var learnerMatcherMock = new Mock<ILearnerMatcher>();
            learnerMatcherMock.Setup(x => x.MatchLearner(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(() =>
                new LearnerMatchResult
                    {DataLockErrorCode = null, Apprenticeships = new List<ApprenticeshipModel>(commitments)});

            var courseValidationMock = new Mock<ICourseValidator>();
            courseValidationMock.Setup(x => x.ValidateCourse(It.IsAny<CollectionPeriod>(), It.IsAny<List<ApprenticeshipModel>>()))
                .ReturnsAsync(() => 
                    new CourseValidationResult{
                        ValidationResults = new List<ValidationResult>
                        {
                            new ValidationResult
                            {
                                DataLockErrorCode = null,
                                ApprenticeshipId = 1,
                                ApprenticeshipPriceEpisodeId = 1,
                                Period = 1
                            }
                        }});

            Mock.Get(dataCacheMock).Setup(x => x.TryGet(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<ApprenticeshipModel>>(true, commitments));

            var testEarning = new ApprenticeshipContractType1EarningEvent
            {
                Learner = new Learner
                {
                    Uln = 123,
                }
            };

            var actual = await new DataLockService(actorService, new ActorId(Guid.Empty), mapper, paymentLoggerMock,
                    commitmentRepositoryMock, dataCacheMock, learnerMatcherMock.Object, courseValidationMock.Object)
                .HandleEarning(testEarning, default(CancellationToken));

            actual.Should().BeOfType<PayableEarningEvent>();
            (actual as PayableEarningEvent).AccountId.Should().Be(456);
        }

        [Test]
        public async Task ThenLearnerDataLockReturned()
        {
            var actorService = MockActorServiceFactory.CreateActorServiceForActor<DataLockService>();
            var paymentLoggerMock = Mock.Of<IPaymentLogger>();
            var commitmentRepositoryMock = Mock.Of<IApprenticeshipRepository>();
            var dataCacheMock = Mock.Of<IActorDataCache<List<ApprenticeshipModel>>>();

            var commitments = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    AccountId = 456,
                }
            };

            var learnerMatcherMock = new Mock<ILearnerMatcher>();
            learnerMatcherMock.Setup(x => x.MatchLearner(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(() =>
                new LearnerMatchResult
                    { DataLockErrorCode = DataLockErrorCode.DLOCK_01, Apprenticeships = new List<ApprenticeshipModel>(commitments) });

            var courseValidationMock = Mock.Of<ICourseValidator>();

            Mock.Get(dataCacheMock).Setup(x => x.TryGet(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<ApprenticeshipModel>>(true, commitments));

            var testEarning = new ApprenticeshipContractType1EarningEvent
            {
                Learner = new Learner
                {
                    Uln = 123,
                }
            };

            var actual = await new DataLockService(actorService, new ActorId(Guid.Empty), mapper, paymentLoggerMock,
                    commitmentRepositoryMock, dataCacheMock, learnerMatcherMock.Object, courseValidationMock)
                .HandleEarning(testEarning, default(CancellationToken));

            actual.Should().BeNull();
        }

        [Test]
        public async Task ThenCourseValidationDataLockReturned()
        {
            var actorService = MockActorServiceFactory.CreateActorServiceForActor<DataLockService>();
            var paymentLoggerMock = Mock.Of<IPaymentLogger>();
            var commitmentRepositoryMock = Mock.Of<IApprenticeshipRepository>();
            var dataCacheMock = Mock.Of<IActorDataCache<List<ApprenticeshipModel>>>();

            var commitments = new List<ApprenticeshipModel>
            {
                new ApprenticeshipModel
                {
                    AccountId = 456,
                }
            };

            var learnerMatcherMock = new Mock<ILearnerMatcher>();
            learnerMatcherMock.Setup(x => x.MatchLearner(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(() =>
                new LearnerMatchResult
                    { DataLockErrorCode = null, Apprenticeships = new List<ApprenticeshipModel>(commitments) });

            var courseValidationMock = new Mock<ICourseValidator>();
            courseValidationMock.Setup(x => x.ValidateCourse(It.IsAny<CollectionPeriod>(), It.IsAny<List<ApprenticeshipModel>>()))
                .ReturnsAsync(() =>
                    new CourseValidationResult
                    {
                        ValidationResults = new List<ValidationResult>
                        {
                            new ValidationResult
                            {
                                DataLockErrorCode = DataLockErrorCode.DLOCK_09,
                                ApprenticeshipId = 1,
                                ApprenticeshipPriceEpisodeId = 1,
                                Period = 1
                            }
                        }
                    });

            Mock.Get(dataCacheMock).Setup(x => x.TryGet(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<ApprenticeshipModel>>(true, commitments));

            var testEarning = new ApprenticeshipContractType1EarningEvent
            {
                Learner = new Learner
                {
                    Uln = 123,
                }
            };

            var actual = await new DataLockService(actorService, new ActorId(Guid.Empty), mapper, paymentLoggerMock,
                    commitmentRepositoryMock, dataCacheMock, learnerMatcherMock.Object, courseValidationMock.Object)
                .HandleEarning(testEarning, default(CancellationToken));

            actual.Should().BeNull();
        }
    }
}
