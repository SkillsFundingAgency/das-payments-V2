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
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.FundingSource.Model;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class RequiredLevyAmountFundingSourceServiceTest
    {
        private static IMapper mapper;

        private AutoMock mocker;
        private Mock<IDataCache<CalculatedRequiredLevyAmount>> eventCacheMock;
        private Mock<IDataCache<List<string>>> keyCacheMock;
        private Mock<IDataCache<bool>> monthEndCacheMock;
        private Mock<ILevyFundingSourceRepository> levyAccountRepositoryMock;
        private Mock<IPaymentProcessor> processorMock;
        private Mock<ILevyBalanceService> levyBalanceServiceMock;
        private Mock<ISortableKeyGenerator> sortableKeysMock;
        private IRequiredLevyAmountFundingSourceService service;
        private Mock<IGenerateSortedPaymentKeys> generateSortedPaymentKeysMock;

        private MapperConfiguration mapperConfiguration;
        
        private Mock<IPaymentLogger> paymentLoggerMock;
        private Mock<IDataCache<LevyAccountModel>> levyAccountCacheMock;
        private Mock<IDataCache<List<EmployerProviderPriorityModel>>> employerProviderPrioritiesMock;
        private Mock<IDataCache<List<string>>> refundSortKeysCacheMock;
        private Mock<IDataCache<List<TransferPaymentSortKeyModel>>> transferPaymentSortKeysCacheMock;
        private Mock<IDataCache<List<RequiredPaymentSortKeyModel>>> requiredPaymentSortKeysCacheMock;

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
            eventCacheMock = mocker.Mock<IDataCache<CalculatedRequiredLevyAmount>>();
            keyCacheMock = mocker.Mock<IDataCache<List<string>>>();
            monthEndCacheMock = mocker.Mock<IDataCache<bool>>();
            levyAccountCacheMock = mocker.Mock<IDataCache<LevyAccountModel>>();
            levyAccountRepositoryMock = mocker.Mock<ILevyFundingSourceRepository>();
            processorMock = mocker.Mock<IPaymentProcessor>();
            levyBalanceServiceMock = mocker.Mock<ILevyBalanceService>();
            paymentLoggerMock = new Mock<IPaymentLogger>(MockBehavior.Loose);
            sortableKeysMock = mocker.Mock<ISortableKeyGenerator>();
            employerProviderPrioritiesMock = mocker.Mock<IDataCache<List<EmployerProviderPriorityModel>>>();

            refundSortKeysCacheMock = mocker.Mock<IDataCache<List<string>>>();
            transferPaymentSortKeysCacheMock = mocker.Mock<IDataCache<List<TransferPaymentSortKeyModel>>>();
            requiredPaymentSortKeysCacheMock = mocker.Mock<IDataCache<List<RequiredPaymentSortKeyModel>>>();
            generateSortedPaymentKeysMock = mocker.Mock<IGenerateSortedPaymentKeys>();

            service = mocker.Create<RequiredLevyAmountFundingSourceService>(
                new NamedParameter("mapper", mapper),
                new NamedParameter("paymentLogger", paymentLoggerMock.Object)
            );
            monthEndCacheMock.Setup(x => x.AddOrReplace(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            levyAccountCacheMock.Setup(c => c.AddOrReplace(CacheKeys.LevyBalanceKey, It.IsAny<LevyAccountModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            levyBalanceServiceMock.SetupGet(svc => svc.RemainingBalance).Returns(100);
            levyBalanceServiceMock.SetupGet(svc => svc.RemainingTransferAllowance).Returns(50);
        }

        [TearDown]
        public void TearDown()
        {
            eventCacheMock.Verify();
            keyCacheMock.Verify();
            processorMock.Verify();
            levyAccountRepositoryMock.Verify();
            sortableKeysMock.Verify();
            levyAccountCacheMock.Verify();
            employerProviderPrioritiesMock.Verify();


            refundSortKeysCacheMock.Verify();
            transferPaymentSortKeysCacheMock.Verify();
            requiredPaymentSortKeysCacheMock.Verify();
        }

        [Test]
        public async Task TestStoresRequiredPayment()
        {
            // arrange
            long expectedUln = 100;
            long expectedUkprn = 100;

            var requiredPaymentEvent = new CalculatedRequiredLevyAmount
            {
                Ukprn = expectedUkprn,
                StartDate = DateTime.Today,
                EventId = Guid.NewGuid(),
                Priority = 1,
                Learner = new Learner
                {
                    Uln = expectedUln
                },
                AccountId = 1,

            };

            eventCacheMock.Setup(c => c.AddOrReplace(requiredPaymentEvent.EventId.ToString(), requiredPaymentEvent, CancellationToken.None)).Returns(Task.CompletedTask)
                .Verifiable();

            requiredPaymentSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.RequiredPaymentKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<RequiredPaymentSortKeyModel>>(false, null))
                .Verifiable();

            requiredPaymentSortKeysCacheMock
                .Setup(c =>
                    c.AddOrReplace(CacheKeys.RequiredPaymentKeyListKey,
                        It.Is<List<RequiredPaymentSortKeyModel>>(list => list.Count == 1 &&
                                                                              list[0].Uln == expectedUln &&
                                                                              list[0].Ukprn == expectedUkprn), CancellationToken.None))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // act
            await service.AddRequiredPayment(requiredPaymentEvent);

            // assert in teardown 
        }

        [Test]
        public async Task TestStoresRefundPayments()
        {
            // arrange
            long expectedUln = 100;
            long expectedUkprn = 100;

            var requiredPaymentEvent = new CalculatedRequiredLevyAmount
            {
                Ukprn = expectedUkprn,
                EventId = Guid.NewGuid(),
                Priority = 1,
                Learner = new Learner
                {
                    Uln = expectedUln
                },
                AccountId = 1,
                AmountDue = -500
            };

            eventCacheMock.Setup(c => c.AddOrReplace(requiredPaymentEvent.EventId.ToString(), requiredPaymentEvent, CancellationToken.None)).Returns(Task.CompletedTask)
                .Verifiable();

            refundSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.RefundPaymentsKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<string>>(false, null)).Verifiable();

            refundSortKeysCacheMock
                .Setup(c => c.AddOrReplace(CacheKeys.RefundPaymentsKeyListKey, It.Is<List<string>>(list => list.Count == 1), CancellationToken.None))
                .Returns(Task.CompletedTask).Verifiable();

            // act
            await service.AddRequiredPayment(requiredPaymentEvent);

            // assert in teardown 
        }

        [Test]
        public async Task TestStoresTransferPayment()
        {
            // arrange
            long expectedUln = 100;

            var requiredPaymentEvent = new CalculatedRequiredLevyAmount
            {

                Ukprn = 100,
                EventId = Guid.NewGuid(),
                Priority = 1,
                Learner = new Learner
                {
                    Uln = expectedUln
                },
                AccountId = 1,
                TransferSenderAccountId = 9
            };

            eventCacheMock.Setup(c => c.AddOrReplace(requiredPaymentEvent.EventId.ToString(), requiredPaymentEvent, CancellationToken.None))
                .Returns(Task.CompletedTask)
                .Verifiable();

            transferPaymentSortKeysCacheMock
                .Setup(c => c.TryGet(CacheKeys.SenderTransferKeyListKey, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<TransferPaymentSortKeyModel>>(false, null)).Verifiable();

            transferPaymentSortKeysCacheMock
                .Setup(c =>
                    c.AddOrReplace(CacheKeys.SenderTransferKeyListKey,
                        It.Is<List<TransferPaymentSortKeyModel>>(list => list.Count == 1 && list[0].Uln == expectedUln), CancellationToken.None))
                .Returns(Task.CompletedTask).Verifiable();

            // act
            await service.AddRequiredPayment(requiredPaymentEvent);

            // assert in teardown 
        }
        
        [Test]
        public async Task TestStoreEmployerProviderPriority()
        {
            // arrange
            var employerChangedProviderPriority = new EmployerChangedProviderPriority
            {
                EmployerAccountId = 1,
                OrderedProviders = new List<long> { 1, 2, 3 }
            };


            levyAccountRepositoryMock
                .Setup(x => x.AddEmployerProviderPriorities(
                    It.Is<List<EmployerProviderPriorityModel>>(o => o.Count == 3 && o[0].EmployerAccountId == 1 && o[0].Order == 1 && o[0].Ukprn == 1),
                    CancellationToken.None))
                .Returns(Task.CompletedTask).Verifiable();

            employerProviderPrioritiesMock
                 .Setup(x => x.AddOrReplace(CacheKeys.EmployerPaymentPriorities,
                     It.Is<List<EmployerProviderPriorityModel>>(o => o.Count == 3 && o[0].EmployerAccountId == 1 && o[0].Order == 1 && o[0].Ukprn == 1),
                     CancellationToken.None))
                 .Returns(Task.CompletedTask).Verifiable();

            // act
            await service.StoreEmployerProviderPriority(employerChangedProviderPriority);

            // assert in tear down
        }
        
        [Test]
        public async Task TestRegisterSubsequentRequiredPayment()
        {
            // arrange
            var keys = new List<string> { "1", "2" };
            var requiredPaymentEvent = new CalculatedRequiredLevyAmount
            {
                EventId = Guid.NewGuid(),
                Priority = 4,
                Learner = new Learner(),
                AccountId = 1
            };

            var key = GenerateSortableKey(requiredPaymentEvent);

            eventCacheMock.Setup(c => c.Add(key, requiredPaymentEvent, CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None)).ReturnsAsync(() => new ConditionalValue<List<string>>(true, keys)).Verifiable();
            keyCacheMock.Setup(c => c.AddOrReplace("keys", It.Is<List<string>>(list => list.Count == 3 && list[2] == key), CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            sortableKeysMock.Setup(x => x.Generate(It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<bool>(), It.IsAny<Guid>()))
                .Returns(key);

            // act
            await service.AddRequiredPayment(requiredPaymentEvent);

            // assert

        }

        [Test]
        public async Task TestProcessRequiredPayments()
        {
            // arrange
            var keys = new List<string> { "1" };
            var requiredPaymentEvent = new CalculatedRequiredLevyAmount
            {
                EventId = Guid.NewGuid(),
                AmountDue = 100,
                SfaContributionPercentage = 11,
                OnProgrammeEarningType = OnProgrammeEarningType.Completion,
                Learner = new Learner(),
            };

            var balance = 100m;
            var transferAllowance = 50;
            var levyPayment = new LevyPayment { AmountDue = 55, Type = FundingSourceType.Levy };
            var employerCoInvestedPayment = new EmployerCoInvestedPayment { AmountDue = 44, Type = FundingSourceType.CoInvestedEmployer };
            var sfaCoInvestedPayment = new SfaCoInvestedPayment { AmountDue = 33, Type = FundingSourceType.CoInvestedSfa };
            var allPayments = new FundingSourcePayment[] { levyPayment, employerCoInvestedPayment, sfaCoInvestedPayment };

            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<string>>(true, keys))
                .Verifiable();

            eventCacheMock.Setup(c => c.TryGet("1", CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<CalculatedRequiredLevyAmount>(true, requiredPaymentEvent))
                .Verifiable();

            levyAccountRepositoryMock.Setup(r => r.GetLevyAccount(666, CancellationToken.None))
                .ReturnsAsync(() => new LevyAccountModel { AccountId = 666, Balance = balance, TransferAllowance = transferAllowance })
                .Verifiable();

            levyBalanceServiceMock.Setup(s => s.Initialise(balance, transferAllowance)).Verifiable();

            processorMock.Setup(p => p.Process(It.IsAny<RequiredPayment>())).Returns(() => allPayments).Verifiable();

            eventCacheMock.Setup(c => c.Clear("1", CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            keyCacheMock.Setup(c => c.Clear("keys", CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            sortableKeysMock.Setup(x => x.Generate(It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<bool>(), It.IsAny<Guid>()))
                .Returns(keys[0]);
            levyAccountCacheMock.Setup(c => c.AddOrReplace(It.Is<string>(key => key.Equals(CacheKeys.LevyBalanceKey)), It.Is<LevyAccountModel>(model => model.AccountId == 666), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // act
            var fundingSourcePayments = await service.HandleMonthEnd(666, 1);

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
        public async Task TestMonthEndPaymentProcessing()
        {
            // arrange
            var keys = new List<string> { "1", "99", "4", "9" };
            var expectedKeys = new Queue<string>(new[] { "1", "4", "9", "99" });
            var expectedKeys2 = new Queue<string>(new[] { "1", "4", "9", "99" });
            var requiredPaymentEvent = new CalculatedRequiredLevyAmount
            {
                EventId = Guid.NewGuid(),
                AmountDue = 100,
                SfaContributionPercentage = 11,
                OnProgrammeEarningType = OnProgrammeEarningType.Completion,
                Priority = 4,
                Learner = new Learner()
            };

            var value = new ConditionalValue<CalculatedRequiredLevyAmount>(true, requiredPaymentEvent);
            var requiredPayments = new Queue<ConditionalValue<CalculatedRequiredLevyAmount>>(new[] { value, value, value, value });
            var fundingSourcePayment = new LevyPayment { AmountDue = 55, Type = FundingSourceType.CoInvestedEmployer };

            var balance = 100m;
            var transferAllowance = 50;

            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<string>>(true, keys))
                .Verifiable();

            eventCacheMock.Setup(c => c.TryGet(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(requiredPayments.Dequeue())
                .Callback<string, CancellationToken>((k, c) => Assert.AreEqual(expectedKeys.Dequeue(), k))
                .Verifiable();

            levyAccountRepositoryMock.Setup(r => r.GetLevyAccount(666, CancellationToken.None))
                .ReturnsAsync(() => new LevyAccountModel { AccountId = 666, Balance = balance, TransferAllowance = transferAllowance })
                .Verifiable();

            levyBalanceServiceMock.Setup(s => s.Initialise(balance, transferAllowance)).Verifiable();

            processorMock.Setup(p => p.Process(It.IsAny<RequiredPayment>())).Returns(() => new[] { fundingSourcePayment }).Verifiable();

            eventCacheMock.Setup(c => c.Clear(It.IsAny<string>(), CancellationToken.None))
                .Returns(Task.CompletedTask)
                .Callback<string, CancellationToken>((k, c) => Assert.AreEqual(expectedKeys2.Dequeue(), k))
                .Verifiable();

            sortableKeysMock.Setup(x => x.Generate(It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<bool>(), It.IsAny<Guid>()))
                .Returns(keys[0]);


            keyCacheMock.Setup(c => c.Clear("keys", CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            levyAccountCacheMock.Setup(c => c.AddOrReplace(CacheKeys.LevyBalanceKey, It.Is<LevyAccountModel>(model => model.AccountId == 666), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // act
            await service.HandleMonthEnd(666, 1);

            // assert
            Assert.AreEqual(0, expectedKeys.Count);
            Assert.AreEqual(0, expectedKeys2.Count);
        }

        private string GenerateSortableKey(CalculatedRequiredLevyAmount requiredPayment)
        {
            return string.Concat(requiredPayment.AmountDue < 0 ? "1" : "9", "-",
                requiredPayment.Priority.ToString("000000"), "-",
                DateTime.MaxValue.ToString("yyyyMMddhhmm"), "-",
                requiredPayment.Learner.Uln);
        }

        [Test]
        public async Task ProcessesTransferPayments()
        {
            // arrange
            var keys = new List<string> { "1" };
            var requiredPaymentEvent = new CalculatedRequiredLevyAmount
            {
                EventId = Guid.NewGuid(),
                AmountDue = 100,
                SfaContributionPercentage = 11,
                OnProgrammeEarningType = OnProgrammeEarningType.Completion,
                Learner = new Learner(),
                AccountId = 2,
                TransferSenderAccountId = 666
            };

            var balance = 100m;
            var transferAllowance = 50;
            var transferPayment = new TransferPayment { AmountDue = 55, Type = FundingSourceType.Transfer };
            var employerCoInvestedPayment = new EmployerCoInvestedPayment { AmountDue = 44, Type = FundingSourceType.CoInvestedEmployer };
            var sfaCoInvestedPayment = new SfaCoInvestedPayment { AmountDue = 33, Type = FundingSourceType.CoInvestedSfa };
            var allPayments = new FundingSourcePayment[] { transferPayment, employerCoInvestedPayment, sfaCoInvestedPayment };

            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<string>>(true, keys))
                .Verifiable();

            eventCacheMock.Setup(c => c.TryGet("1", CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<CalculatedRequiredLevyAmount>(true, requiredPaymentEvent))
                .Verifiable();

            levyAccountRepositoryMock.Setup(r => r.GetLevyAccount(666, CancellationToken.None))
                .ReturnsAsync(() => new LevyAccountModel { Balance = balance, TransferAllowance = transferAllowance })
                .Verifiable();

            levyBalanceServiceMock.Setup(s => s.Initialise(balance, transferAllowance)).Verifiable();

            processorMock.Setup(p => p.Process(It.IsAny<RequiredPayment>())).Returns(() => allPayments);

            eventCacheMock.Setup(c => c.Clear("1", CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            keyCacheMock.Setup(c => c.Clear("keys", CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            sortableKeysMock.Setup(x => x.Generate(It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<bool>(), It.IsAny<Guid>()))
                .Returns(keys[0]);

            // act
            var fundingSourcePayments = await service.HandleMonthEnd(666, 1);

            processorMock.Verify(p => p.Process(It.Is<RequiredPayment>(rp => rp.IsTransfer)), Times.Once);

            // assert
            fundingSourcePayments.Should().HaveCount(3);
            fundingSourcePayments[0].Should().BeOfType<TransferFundingSourcePaymentEvent>();
            fundingSourcePayments[1].Should().BeOfType<EmployerCoInvestedFundingSourcePaymentEvent>();
            fundingSourcePayments[2].Should().BeOfType<SfaCoInvestedFundingSourcePaymentEvent>();

            fundingSourcePayments[0].AmountDue.Should().Be(55);
            fundingSourcePayments[1].AmountDue.Should().Be(44);
            fundingSourcePayments[2].AmountDue.Should().Be(33);
        }

        [Test]
        public async Task ProcessesUnableToFundTransferPayments()
        {
            // arrange
            var keys = new List<string> { "1" };
            var requiredPaymentEvent = new CalculatedRequiredLevyAmount
            {
                EventId = Guid.NewGuid(),
                AmountDue = 100,
                SfaContributionPercentage = 11,
                OnProgrammeEarningType = OnProgrammeEarningType.Completion,
                Learner = new Learner(),
                AccountId = 666,
                TransferSenderAccountId = 2
            };

            var balance = 100m;
            var transferAllowance = 50;
            var transferPayment = new TransferPayment { AmountDue = 55, Type = FundingSourceType.Transfer };
            var unableToFundTransferPayment = new UnableToFundTransferPayment { AmountDue = 44, Type = FundingSourceType.Transfer };
            var allPayments = new FundingSourcePayment[] { transferPayment, unableToFundTransferPayment };

            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<string>>(true, keys))
                .Verifiable();

            eventCacheMock.Setup(c => c.TryGet("1", CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<CalculatedRequiredLevyAmount>(true, requiredPaymentEvent))
                .Verifiable();

            levyAccountRepositoryMock.Setup(r => r.GetLevyAccount(666, CancellationToken.None))
                .ReturnsAsync(() => new LevyAccountModel { Balance = balance, TransferAllowance = transferAllowance })
                .Verifiable();

            levyBalanceServiceMock.Setup(s => s.Initialise(balance, transferAllowance)).Verifiable();

            processorMock.Setup(p => p.Process(It.IsAny<RequiredPayment>())).Returns(() => allPayments).Verifiable();

            eventCacheMock.Setup(c => c.Clear("1", CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();
            keyCacheMock.Setup(c => c.Clear("keys", CancellationToken.None)).Returns(Task.CompletedTask).Verifiable();








            sortableKeysMock.Setup(x => x.Generate(It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<bool>(), It.IsAny<Guid>()))
                .Returns(keys[0]);

            // act
            var fundingSourcePayments = await service.HandleMonthEnd(666, 1);

            // assert
            fundingSourcePayments.Should().HaveCount(2);
            fundingSourcePayments[0].Should().BeOfType<TransferFundingSourcePaymentEvent>();
            fundingSourcePayments[1].Should().BeOfType<ProcessUnableToFundTransferFundingSourcePayment>();

            fundingSourcePayments[0].AmountDue.Should().Be(55);
            fundingSourcePayments[1].AmountDue.Should().Be(44);
        }


        [Test]
        public async Task ShouldSetMonthEndPaymentsProcessedFlag()
        {
            // arrange
            var keys = new List<string> { "1" };
            var requiredPaymentEvent = new CalculatedRequiredLevyAmount
            {
                EventId = Guid.NewGuid(),
                AmountDue = 100,
                SfaContributionPercentage = 11,
                OnProgrammeEarningType = OnProgrammeEarningType.Completion,
                Learner = new Learner(),
            };

            var balance = 100m;
            var transferAllowance = 50;
            var levyPayment = new LevyPayment { AmountDue = 55, Type = FundingSourceType.Levy };
            var employerCoInvestedPayment = new EmployerCoInvestedPayment { AmountDue = 44, Type = FundingSourceType.CoInvestedEmployer };
            var sfaCoInvestedPayment = new SfaCoInvestedPayment { AmountDue = 33, Type = FundingSourceType.CoInvestedSfa };
            var allPayments = new FundingSourcePayment[] { levyPayment, employerCoInvestedPayment, sfaCoInvestedPayment };

            keyCacheMock.Setup(c => c.TryGet("keys", CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<string>>(true, keys))
                .Verifiable();

            eventCacheMock.Setup(c => c.TryGet("1", CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<CalculatedRequiredLevyAmount>(true, requiredPaymentEvent))
                .Verifiable();

            levyAccountRepositoryMock.Setup(r => r.GetLevyAccount(666, CancellationToken.None))
                .ReturnsAsync(() => new LevyAccountModel { Balance = balance, TransferAllowance = transferAllowance })
                .Verifiable();

            levyBalanceServiceMock.Setup(s => s.Initialise(balance, transferAllowance)).Verifiable();

            processorMock.Setup(p => p.Process(It.IsAny<RequiredPayment>())).Returns(() => allPayments).Verifiable();

            eventCacheMock.Setup(c => c.Clear(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            keyCacheMock.Setup(c => c.Clear(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            sortableKeysMock.Setup(x => x.Generate(It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<bool>(), It.IsAny<Guid>()))
                .Returns(keys[0]);

            // act
            await service.HandleMonthEnd(666, 1);

            // assert
            monthEndCacheMock.Verify(x => x.AddOrReplace(It.Is<string>(key => key.Equals(CacheKeys.MonthEndCacheKey)), It.Is<bool>(value => value), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ShouldAddPaymentToCacheIfMonthEndNotStarted()
        {
            // arrange
            var keys = new List<string> { "1", "2" };
            var unableToFundEvent = new ProcessUnableToFundTransferFundingSourcePayment
            {
                EventId = Guid.NewGuid(),
                Learner = new Learner(),
                AccountId = 1,
                AmountDue = 100
            };

            var sortableKey = "test_key";

            eventCacheMock.Setup(c => c.Add(sortableKey, It.Is<CalculatedRequiredLevyAmount>(levyEvent => levyEvent.AmountDue == unableToFundEvent.AmountDue && levyEvent.EventId == unableToFundEvent.EventId), CancellationToken.None))
                .Returns(Task.CompletedTask)
                .Verifiable();
            keyCacheMock.Setup(c => c.TryGet(CacheKeys.KeyListKey, CancellationToken.None))
                .ReturnsAsync(() => new ConditionalValue<List<string>>(true, keys))
                .Verifiable();
            keyCacheMock.Setup(c => c.AddOrReplace(CacheKeys.KeyListKey, It.Is<List<string>>(list => list.Count == 3 && list[2] == sortableKey), CancellationToken.None))
                .Returns(Task.CompletedTask)
                .Verifiable();
            sortableKeysMock.Setup(x => x.Generate(It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<bool>(), It.IsAny<Guid>()))
                .Returns(sortableKey);
            monthEndCacheMock.Setup(c => c.TryGet(It.Is<string>(key => key.Equals(CacheKeys.MonthEndCacheKey)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new ConditionalValue<bool>(false, false))
                .Verifiable();

            // act
            await service.ProcessReceiverTransferPayment(unableToFundEvent);

            // assert
        }

        [Test]
        public async Task ShouldProcessPaymentIfMonthEndProcessed()
        {
            // arrange
            var keys = new List<string> { "1" };
            var unableToFundEvent = new ProcessUnableToFundTransferFundingSourcePayment
            {
                EventId = Guid.NewGuid(),
                Learner = new Learner(),
                AccountId = 666,
                AmountDue = 100
            };

            var balance = 100m;
            var transferAllowance = 50;
            var levyPayment = new LevyPayment { AmountDue = 55, Type = FundingSourceType.Levy };
            var employerCoInvestedPayment = new EmployerCoInvestedPayment { AmountDue = 44, Type = FundingSourceType.CoInvestedEmployer };
            var sfaCoInvestedPayment = new SfaCoInvestedPayment { AmountDue = 33, Type = FundingSourceType.CoInvestedSfa };
            var allPayments = new FundingSourcePayment[] { levyPayment, employerCoInvestedPayment, sfaCoInvestedPayment };

            levyBalanceServiceMock.Setup(s => s.Initialise(balance, transferAllowance))
                .Verifiable();

            processorMock.Setup(p => p.Process(It.IsAny<RequiredPayment>()))
                .Returns(() => allPayments)
                .Verifiable();

            monthEndCacheMock.Setup(c => c.TryGet(It.Is<string>(key => key.Equals(CacheKeys.MonthEndCacheKey)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new ConditionalValue<bool>(true, true))
                .Verifiable();

            levyAccountCacheMock.Setup(c => c.TryGet(CacheKeys.LevyBalanceKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new ConditionalValue<LevyAccountModel>(true, new LevyAccountModel { AccountId = 666, Balance = balance, TransferAllowance = transferAllowance }))
                .Verifiable();

            levyAccountCacheMock.Setup(c => c.AddOrReplace(CacheKeys.LevyBalanceKey, It.Is<LevyAccountModel>(model => model.AccountId == 666), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // act
            var fundingSourcePayments = await service.ProcessReceiverTransferPayment(unableToFundEvent);

            // assert
            fundingSourcePayments.Should().HaveCount(3);
            fundingSourcePayments[0].Should().BeOfType<LevyFundingSourcePaymentEvent>();
            fundingSourcePayments[1].Should().BeOfType<EmployerCoInvestedFundingSourcePaymentEvent>();
            fundingSourcePayments[2].Should().BeOfType<SfaCoInvestedFundingSourcePaymentEvent>();

            fundingSourcePayments[0].AmountDue.Should().Be(55);
            fundingSourcePayments[1].AmountDue.Should().Be(44);
            fundingSourcePayments[2].AmountDue.Should().Be(33);
        }
    }
}