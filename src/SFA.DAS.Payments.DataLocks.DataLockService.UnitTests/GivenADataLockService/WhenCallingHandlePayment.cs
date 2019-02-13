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
            var commitmentRepositoryMock = Mock.Of<ICommitmentRepository>();
            var dataCacheMock = Mock.Of<IDataCache<List<CommitmentModel>>>();

            var commitments = new List<CommitmentModel>
            {
                new CommitmentModel
                {
                    AccountId = 456,
                }
            };

            Mock.Get(dataCacheMock).Setup(x => x.TryGet(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<CommitmentModel>>(true, commitments));

            var testEarning = new ApprenticeshipContractType1EarningEvent
            {
                Learner = new Learner
                {
                    Uln = 123,
                }
            };

            var actual = await (new DataLockService(actorService, new ActorId(Guid.Empty), mapper, paymentLoggerMock,
                    commitmentRepositoryMock, dataCacheMock))
                .HandleEarning(testEarning, default(CancellationToken));

            actual.Should().BeOfType<PayableEarningEvent>();
            (actual as PayableEarningEvent).EmployerAccountId.Should().Be(456);
        }
    }
}
