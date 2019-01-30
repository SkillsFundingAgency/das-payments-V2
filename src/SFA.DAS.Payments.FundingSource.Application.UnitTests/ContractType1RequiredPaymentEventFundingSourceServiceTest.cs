using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests
{
    [TestFixture]
    public class ContractType1RequiredPaymentEventFundingSourceServiceTest
    {
        private AutoMock mocker;
        private Mock<IDataCache<ApprenticeshipContractType1RequiredPaymentEvent>> eventCacheMock;
        private Mock<IDataCache<List<string>>> keyCacheMock;
        private Mock<ILevyAccountRepository> levyAccountRepositoryMock;
        private Mock<ILevyPaymentProcessor> processor1Mock;
        private Mock<ILevyPaymentProcessor> processor2Mock;
        private Mock<ILevyFundingSourcePaymentEventMapper> mapperMock;
        private IContractType1RequiredPaymentEventFundingSourceService service;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetStrict();
            eventCacheMock = mocker.Mock<IDataCache<ApprenticeshipContractType1RequiredPaymentEvent>>();
            keyCacheMock = mocker.Mock<IDataCache<List<string>>>();
            levyAccountRepositoryMock = mocker.Mock<ILevyAccountRepository>();
            processor1Mock = mocker.Mock<ILevyPaymentProcessor>();
            processor2Mock = mocker.Mock<ILevyPaymentProcessor>();
            mapperMock = mocker.Mock<ILevyFundingSourcePaymentEventMapper>();
            var processorsParam = new[] {processor1Mock.Object, processor2Mock.Object};
            service = mocker.Create<ContractType1RequiredPaymentEventFundingSourceService>(
                new NamedParameter("processors", processorsParam),
                new NamedParameter("employerAccountId", 666)
            );
        }

        [TearDown]
        public void TearDown()
        {
            eventCacheMock.Verify();
            keyCacheMock.Verify();
        }

        [Test]
        public async Task TestRegisterFirstRequiredPayment()
        {
            // arrange
            var requiredPaymentEvent = new ApprenticeshipContractType1RequiredPaymentEvent
            {
                EventId = Guid.NewGuid()
            };
            eventCacheMock.Setup(c => c.Add(requiredPaymentEvent.EventId.ToString(), requiredPaymentEvent, CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None)).ReturnsAsync(() => new ConditionalValue<List<string>>(false, null)).Verifiable();
            keyCacheMock.Setup(c => c.AddOrReplace("keys", It.Is<List<string>>(list => list.Count == 1 && list[0] == requiredPaymentEvent.EventId.ToString()), CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();

            // act
            await service.RegisterRequiredPayment(requiredPaymentEvent);

            // assert
        }

        [Test]
        public async Task TestRegisterSubsequentRequiredPayment()
        {
            // arrange
            var keys = new List<string> {"1", "2"};
            var requiredPaymentEvent = new ApprenticeshipContractType1RequiredPaymentEvent
            {
                EventId = Guid.NewGuid()
            };
            eventCacheMock.Setup(c => c.Add(requiredPaymentEvent.EventId.ToString(), requiredPaymentEvent, CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None)).ReturnsAsync(() => new ConditionalValue<List<string>>(true, keys)).Verifiable();
            keyCacheMock.Setup(c => c.AddOrReplace("keys", It.Is<List<string>>(list => list.Count == 3 && list[2] == requiredPaymentEvent.EventId.ToString()), CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();

            // act
            await service.RegisterRequiredPayment(requiredPaymentEvent);

            // assert
        }

        [Test]
        public async Task TestProcessRequiredPayments()
        {
            // arrange
            var keys = new List<string> {"1"};
            var requiredPaymentEvent = new ApprenticeshipContractType1RequiredPaymentEvent {EventId = Guid.NewGuid(), AmountDue = 50};
            var balance = 100m;

            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None)).ReturnsAsync(() => new ConditionalValue<List<string>>(true, keys)).Verifiable();
            eventCacheMock.Setup(c => c.TryGet("1", CancellationToken.None)).ReturnsAsync(() => new ConditionalValue<ApprenticeshipContractType1RequiredPaymentEvent>(true, requiredPaymentEvent)).Verifiable();

            levyAccountRepositoryMock.Setup(r => r.GetLevyAccount(666, CancellationToken.None)).ReturnsAsync(() => new LevyAccountModel {Balance = balance}).Verifiable();
            processor1Mock.Setup(p => p.Process(It.IsAny<RequiredLevyPayment>(), ref balance)).Returns(() => new LevyPayment {AmountDue = 50, Type = FundingSourceType.Levy}).Verifiable();
            processor2Mock.Setup(p => p.Process(It.IsAny<RequiredLevyPayment>(), ref balance)).Returns(() => new EmployerCoInvestedPayment {AmountDue = 50, Type = FundingSourceType.CoInvestedEmployer}).Verifiable();

            mapperMock.Setup(m => m.MapToFundingSourcePaymentEvent(It.IsAny<FundingSourcePayment>())).Returns(new LevyFundingSourcePaymentEvent{ FundingSourceType = FundingSourceType.Levy, AmountDue = 50 }).Verifiable();

            eventCacheMock.Setup(c => c.Clear("1", CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            keyCacheMock.Setup(c => c.Clear("keys", CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();

            // act
            var fundingSourcePayments = await service.GetFundedPayments();

            // assert
            Assert.AreEqual(2, fundingSourcePayments.Count);
            Assert.AreEqual(50, fundingSourcePayments.First().AmountDue);
            Assert.AreEqual(FundingSourceType.Levy, fundingSourcePayments.First().FundingSourceType);
        }
    }
}