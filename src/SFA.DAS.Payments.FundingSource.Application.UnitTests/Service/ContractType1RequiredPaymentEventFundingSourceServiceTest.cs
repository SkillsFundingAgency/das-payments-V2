using System;
using System.Collections.Generic;
using System.Linq;
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
        private Mock<ILevyPaymentProcessor> levyProcessorMock;
        private Mock<ICoInvestedPaymentProcessor> coInvestedProcessorMock;
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
            levyProcessorMock = mocker.Mock<ILevyPaymentProcessor>();
            coInvestedProcessorMock = mocker.Mock<ICoInvestedPaymentProcessor>();
            var processorsParam = new IPaymentProcessor[] {levyProcessorMock.Object, coInvestedProcessorMock.Object};
            service = mocker.Create<ContractType1RequiredPaymentEventFundingSourceService>(
                new NamedParameter("processors", processorsParam),
                new NamedParameter("employerAccountId", 666),
                new NamedParameter("mapper", mapper)
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
                EventId = Guid.NewGuid(),
                Priority = 1
            };

            var key = string.Concat("000001-000000-", requiredPaymentEvent.EventId.ToString());

            eventCacheMock.Setup(c => c.Add(key, requiredPaymentEvent, CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None)).ReturnsAsync(() => new ConditionalValue<List<string>>(false, null)).Verifiable();
            keyCacheMock.Setup(c => c.AddOrReplace("keys", It.Is<List<string>>(list => list.Count == 1 && list[0] == key), CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();

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
                EventId = Guid.NewGuid(),
                Priority = 4
            };

            var key = string.Concat("000004-000002-", requiredPaymentEvent.EventId.ToString());

            eventCacheMock.Setup(c => c.Add(key, requiredPaymentEvent, CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None)).ReturnsAsync(() => new ConditionalValue<List<string>>(true, keys)).Verifiable();
            keyCacheMock.Setup(c => c.AddOrReplace("keys", It.Is<List<string>>(list => list.Count == 3 && list[2] == key), CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();

            // act
            await service.RegisterRequiredPayment(requiredPaymentEvent);

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

            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<string>>(true, keys))
                .Verifiable();

            eventCacheMock.Setup(c => c.TryGet("1", CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<ApprenticeshipContractType1RequiredPaymentEvent>(true, requiredPaymentEvent))
                .Verifiable();

            levyAccountRepositoryMock.Setup(r => r.GetLevyAccount(666, CancellationToken.None))
                .ReturnsAsync(() => new LevyAccountModel {Balance = balance})
                .Verifiable();

            levyProcessorMock.Setup(p => p.Process(It.Is<RequiredLevyPayment>(payment => payment.AmountFunded == 0 && payment.LevyBalance == balance)))
                .Returns(() => new LevyPayment {AmountDue = 45, Type = FundingSourceType.Levy})
                .Verifiable();

            coInvestedProcessorMock.Setup(p => p.Process(It.IsAny<RequiredLevyPayment>()))
                .Returns(() => new EmployerCoInvestedPayment {AmountDue = 55, Type = FundingSourceType.CoInvestedEmployer})
                .Verifiable();

            eventCacheMock.Setup(c => c.Clear("1", CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            keyCacheMock.Setup(c => c.Clear("keys", CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();

            // act
            var fundingSourcePayments = await service.GetFundedPayments(666);

            // assert
            fundingSourcePayments.Should().HaveCount(2);

            var levyPayment = fundingSourcePayments.First();
            levyPayment.AmountDue.Should().Be(45);
            levyPayment.FundingSourceType.Should().Be(FundingSourceType.Levy);
            levyPayment.ContractType.Should().Be(1);
            levyPayment.SfaContributionPercentage.Should().Be(11);
            levyPayment.TransactionType.Should().Be(TransactionType.Completion);

            var employerPayment = fundingSourcePayments.Last();
            employerPayment.AmountDue.Should().Be(55);
            employerPayment.FundingSourceType.Should().Be(FundingSourceType.CoInvestedEmployer);
            employerPayment.ContractType.Should().Be(1);
            employerPayment.SfaContributionPercentage.Should().Be(11);
            employerPayment.TransactionType.Should().Be(TransactionType.Completion);
        }

        [Test]
        public async Task TestRequiredPaymentsSorted()
        {
            // arrange
            var keys = new List<string> {"1", "99", "4", "9"};
            var expectedKeys = new Queue<string>(new[] {"1", "4", "9", "99"});
            var expectedKeys2 = new Queue<string>(new[] {"1", "4", "9", "99"});
            var requiredPaymentEvent = CreateRequiredPayment(4);
            var value = new ConditionalValue<ApprenticeshipContractType1RequiredPaymentEvent>(true, requiredPaymentEvent);
            var requiredPayments = new Queue<ConditionalValue<ApprenticeshipContractType1RequiredPaymentEvent>>(new[] {value, value, value, value});

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

            levyProcessorMock.Setup(p => p.Process(It.Is<RequiredLevyPayment>(payment => payment.AmountFunded == 0 && payment.LevyBalance == balance)))
                .Returns(() => new LevyPayment {AmountDue = 45, Type = FundingSourceType.Levy})
                .Verifiable();

            coInvestedProcessorMock.Setup(p => p.Process(It.IsAny<RequiredLevyPayment>()))
                .Returns(() => new EmployerCoInvestedPayment {AmountDue = 55, Type = FundingSourceType.CoInvestedEmployer})
                .Verifiable();

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

        private static ApprenticeshipContractType1RequiredPaymentEvent CreateRequiredPayment(int priority)
        {
            return new ApprenticeshipContractType1RequiredPaymentEvent
            {
                EventId = Guid.NewGuid(), 
                AmountDue = 100,
                SfaContributionPercentage = 11,
                OnProgrammeEarningType = OnProgrammeEarningType.Completion,
                Priority = priority
            };
        }
    }
}