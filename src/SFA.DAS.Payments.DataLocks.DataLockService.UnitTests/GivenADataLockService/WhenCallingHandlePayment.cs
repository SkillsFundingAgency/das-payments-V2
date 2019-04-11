using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Application.Mapping;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
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

            var testEarning = new ApprenticeshipContractType1EarningEvent
            {
                Learner = new Learner
                {
                    Uln = 123,
                }
            };

            var dataLockProcessor = new Mock<IDataLockProcessor>();
            dataLockProcessor.Setup(x =>
                    x.Validate(It.IsAny<ApprenticeshipContractType1EarningEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new List<DataLockEvent>
                    { new PayableEarningEvent {AccountId = 456}});

            var actual = await new DataLockService(actorService, new ActorId(Guid.Empty), paymentLoggerMock,
                    commitmentRepositoryMock, dataCacheMock, dataLockProcessor.Object)
                .HandleEarning(testEarning, default(CancellationToken));

            actual.Count.Should().Be(1);
            var payable = actual.First();
            payable.Should().BeOfType<PayableEarningEvent>();
            (payable as PayableEarningEvent).AccountId.Should().Be(456);
        }

        [Test]
        public async Task ThenANonPayableEarningEventAreReceived()
        {
            var actorService = MockActorServiceFactory.CreateActorServiceForActor<DataLockService>();
            var paymentLoggerMock = Mock.Of<IPaymentLogger>();
            var commitmentRepositoryMock = Mock.Of<IApprenticeshipRepository>();
            var dataCacheMock = Mock.Of<IActorDataCache<List<ApprenticeshipModel>>>();

            var testEarning = new ApprenticeshipContractType1EarningEvent
            {
                Learner = new Learner
                {
                    Uln = 123,
                }
            };

            var dataLockProcessor = new Mock<IDataLockProcessor>();
            dataLockProcessor.Setup(x =>
                    x.Validate(It.IsAny<ApprenticeshipContractType1EarningEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new List<DataLockEvent>
                {
                    new NonPayableEarningEvent
                    {
                        Errors = new ReadOnlyCollection<DataLockErrorCode>(
                            new List<DataLockErrorCode> {DataLockErrorCode.DLOCK_01})
                    }
                });

            var actual = await new DataLockService(actorService, new ActorId(Guid.Empty), paymentLoggerMock,
                    commitmentRepositoryMock, dataCacheMock, dataLockProcessor.Object)
                .HandleEarning(testEarning, default(CancellationToken));

            actual.Count.Should().Be(1);
            var nonPayable = actual.First();
            nonPayable.Should().BeOfType<NonPayableEarningEvent>();
            (nonPayable as NonPayableEarningEvent).Errors.Count.Should().Be(1);
            (nonPayable as NonPayableEarningEvent).Errors.First().Should().Be(DataLockErrorCode.DLOCK_01);
        }
    }
}
