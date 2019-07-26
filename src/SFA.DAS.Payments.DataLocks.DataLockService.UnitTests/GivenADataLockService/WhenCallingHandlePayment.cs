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
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Application.Mapping;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
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
            var commitmentRepositoryMock = new Mock<IApprenticeshipRepository>();
            var dataCacheMock = Mock.Of<IActorDataCache<List<ApprenticeshipModel>>>();
            var providersCache = Mock.Of<IActorDataCache<List<long>>>();
            var testEarning = new ApprenticeshipContractType1EarningEvent
            {
                Learner = new Learner
                {
                    Uln = 123456,
                    ReferenceNumber = "1234567",
                    
                },
                CollectionPeriod = new CollectionPeriod { AcademicYear = 1819, Period = 10 }

            };

            var dataLockProcessor = new Mock<IDataLockProcessor>();
            dataLockProcessor.Setup(x => x.GetPaymentEvents(It.IsAny<ApprenticeshipContractType1EarningEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new List<DataLockEvent>
                {
                    new PayableEarningEvent(),
                    new EarningFailedDataLockMatching()
                });

            //commitmentRepositoryMock
            //    .Setup(o => o.ApprenticeshipUlnsByProvider(It.IsAny<long>()))
            //    .ReturnsAsync(new List<ApprenticeshipModel>());

            var actuals = await new DataLockService(actorService, new ActorId(1000),
                    paymentLoggerMock,() =>
                    commitmentRepositoryMock.Object, dataCacheMock,
                    dataLockProcessor.Object,
                    Mock.Of<IApprenticeshipUpdatedProcessor>())
                .HandleEarning(testEarning, default(CancellationToken));

            actuals.Should().HaveCount(2);
            actuals[0].Should().BeOfType<PayableEarningEvent>();
            actuals[1].Should().BeOfType<EarningFailedDataLockMatching>();
        }
    }
}
