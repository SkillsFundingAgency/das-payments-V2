using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class ContractType1RequiredPaymentEventFundingSourceServiceTest
    {
        private static IMapper mapper;

        private AutoMock mocker;
        private Mock<IDataCache<ApprenticeshipContractType1RequiredPaymentEvent>> eventCacheMock;
        private Mock<IDataCache<List<string>>> keyCacheMock;
        private Mock<ILevyAccountRepository> levyAccountRepositoryMock;
        private Mock<ILevyPaymentProcessor> processorMock;
        private Mock<ILevyBalanceService> levyBalanceServiceMock;
        private IContractType1RequiredPaymentEventFundingSourceService service;
        private MapperConfiguration mapperConfiguration;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            mapperConfiguration = AutoMapperConfigurationFactory.CreateMappingConfig();
            mapper = mapperConfiguration.CreateMapper();
        }

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetStrict();
            eventCacheMock = mocker.Mock<IDataCache<ApprenticeshipContractType1RequiredPaymentEvent>>();
            keyCacheMock = mocker.Mock<IDataCache<List<string>>>();
            levyAccountRepositoryMock = mocker.Mock<ILevyAccountRepository>();
            processorMock = mocker.Mock<ILevyPaymentProcessor>();
            levyBalanceServiceMock = mocker.Mock<ILevyBalanceService>();
            service = mocker.Create<ContractType1RequiredPaymentEventFundingSourceService>(
                new NamedParameter("mapper", mapper)
            );
        }

        [TearDown]
        public void TearDown()
        {
            eventCacheMock.Verify();
            keyCacheMock.Verify();
            processorMock.Verify();
            levyAccountRepositoryMock.Verify();
        }

        [Test]
        public async Task TestRegisterFirstRequiredPayment()
        {
            // arrange
            var requiredPaymentEvent = new ApprenticeshipContractType1RequiredPaymentEvent
            {
                EventId = Guid.NewGuid(),
                Priority = 1
            };

            var key = string.Concat("000001-000000-", requiredPaymentEvent.EventId.ToString());

            eventCacheMock.Setup(c => c.Add(key, requiredPaymentEvent, CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None)).ReturnsAsync(() => new ConditionalValue<List<string>>(false, null)).Verifiable();
            keyCacheMock.Setup(c => c.AddOrReplace("keys", It.Is<List<string>>(list => list.Count == 1 && list[0] == key), CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();

            // act
            await service.AddRequiredPayment(requiredPaymentEvent);

            // assert
        }

        [Test]
        public async Task TestRegisterSubsequentRequiredPayment()
        {
            // arrange
            var keys = new List<string> {"1", "2"};
            var requiredPaymentEvent = new ApprenticeshipContractType1RequiredPaymentEvent
            {
                EventId = Guid.NewGuid(),
                Priority = 4
            };

            var key = string.Concat("000004-000002-", requiredPaymentEvent.EventId.ToString());

            eventCacheMock.Setup(c => c.Add(key, requiredPaymentEvent, CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None)).ReturnsAsync(() => new ConditionalValue<List<string>>(true, keys)).Verifiable();
            keyCacheMock.Setup(c => c.AddOrReplace("keys", It.Is<List<string>>(list => list.Count == 3 && list[2] == key), CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();

            // act
            await service.AddRequiredPayment(requiredPaymentEvent);

            // assert
        }

        [Test]
        public async Task TestProcessRequiredPayments()
        {
            // arrange
            var keys = new List<string> {"1"};
            var requiredPaymentEvent = new ApprenticeshipContractType1RequiredPaymentEvent
            {
                EventId = Guid.NewGuid(), 
                AmountDue = 100,
                SfaContributionPercentage = 11,
                OnProgrammeEarningType = OnProgrammeEarningType.Completion
            };
            var balance = 100m;
            var levyPayment = new LevyPayment {AmountDue = 55, Type = FundingSourceType.Levy};
            var employerCoInvestedPayment = new EmployerCoInvestedPayment {AmountDue = 44, Type = FundingSourceType.CoInvestedEmployer};
            var sfaCoInvestedPayment = new SfaCoInvestedPayment() {AmountDue = 33, Type = FundingSourceType.CoInvestedSfa};
            var allPayments = new FundingSourcePayment[] {levyPayment, employerCoInvestedPayment, sfaCoInvestedPayment};


            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<string>>(true, keys))
                .Verifiable();

            eventCacheMock.Setup(c => c.TryGet("1", CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<ApprenticeshipContractType1RequiredPaymentEvent>(true, requiredPaymentEvent))
                .Verifiable();

            levyAccountRepositoryMock.Setup(r => r.GetLevyAccount(666, CancellationToken.None))
                .ReturnsAsync(() => new LevyAccountModel {Balance = balance})
                .Verifiable();

            levyBalanceServiceMock.Setup(s => s.Initialise(balance)).Verifiable();

            processorMock.Setup(p => p.Process(It.IsAny<RequiredPayment>())).Returns(() => allPayments).Verifiable();

            eventCacheMock.Setup(c => c.Clear("1", CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            keyCacheMock.Setup(c => c.Clear("keys", CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();

            // act
            var fundingSourcePayments = await service.GetFundedPayments(666);

            // assert
            fundingSourcePayments.Should().HaveCount(3);
            fundingSourcePayments[0].Should().BeOfType<LevyFundingSourcePaymentEvent>();
            fundingSourcePayments[1].Should().BeOfType<EmployerCoInvestedFundingSourcePaymentEvent>();
            fundingSourcePayments[2].Should().BeOfType<SfaCoInvestedFundingSourcePaymentEvent>();

            fundingSourcePayments[0].AmountDue.Should().Be(55);
            fundingSourcePayments[1].AmountDue.Should().Be(44);
            fundingSourcePayments[2].AmountDue.Should().Be(33);
        }

        [Test]
        public async Task TestRequiredPaymentsSorted()
        {
            // arrange
            var keys = new List<string> {"1", "99", "4", "9"};
            var expectedKeys = new Queue<string>(new[] {"1", "4", "9", "99"});
            var expectedKeys2 = new Queue<string>(new[] {"1", "4", "9", "99"});
            var requiredPaymentEvent = new ApprenticeshipContractType1RequiredPaymentEvent
            {
                EventId = Guid.NewGuid(), 
                AmountDue = 100,
                SfaContributionPercentage = 11,
                OnProgrammeEarningType = OnProgrammeEarningType.Completion,
                Priority = 4
            };
            var value = new ConditionalValue<ApprenticeshipContractType1RequiredPaymentEvent>(true, requiredPaymentEvent);
            var requiredPayments = new Queue<ConditionalValue<ApprenticeshipContractType1RequiredPaymentEvent>>(new[] {value, value, value, value});
            var fundingSourcePayment = new LevyPayment {AmountDue = 55, Type = FundingSourceType.CoInvestedEmployer};

            var balance = 100m;

            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<string>>(true, keys))
                .Verifiable();

            eventCacheMock.Setup(c => c.TryGet(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(requiredPayments.Dequeue())
                .Callback<string, CancellationToken>((k, c) => Assert.AreEqual(expectedKeys.Dequeue(), k))
                .Verifiable();

            levyAccountRepositoryMock.Setup(r => r.GetLevyAccount(666, CancellationToken.None))
                .ReturnsAsync(() => new LevyAccountModel {Balance = balance})
                .Verifiable();

            levyBalanceServiceMock.Setup(s => s.Initialise(balance)).Verifiable();

            processorMock.Setup(p => p.Process(It.IsAny<RequiredPayment>())).Returns(() => new[] {fundingSourcePayment}).Verifiable();

            eventCacheMock.Setup(c => c.Clear(It.IsAny<string>(), CancellationToken.None))                
                .Returns(Task.CompletedTask)
                .Callback<string, CancellationToken>((k, c) => Assert.AreEqual(expectedKeys2.Dequeue(), k))                
                .Verifiable();

            keyCacheMock.Setup(c => c.Clear("keys", CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();

            // act
            await service.GetFundedPayments(666);

            // assert
            Assert.AreEqual(0, expectedKeys.Count);
            Assert.AreEqual(0, expectedKeys2.Count);
        }
    }
}