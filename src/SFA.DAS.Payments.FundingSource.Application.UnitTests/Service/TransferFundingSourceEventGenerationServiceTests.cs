using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class TransferFundingSourceEventGenerationServiceTests
    {
        private Mock<IPaymentLogger> paymentLogger;
        private Mock<IMapper> mapper;
        private Mock<IDataCache<bool>> monthEndCache;
        private Mock<IDataCache<LevyAccountModel>> levyAccountCache;
        private Mock<ILevyBalanceService> levyBalanceService;
        private Mock<IFundingSourcePaymentEventBuilder> fundingSourcePaymentEventBuilder;
        private Mock<ILevyTransactionBatchStorageService> levyTransactionBatchStorageService;

        private TransferFundingSourceEventGenerationService service;

        private ProcessUnableToFundTransferFundingSourcePayment message;
        private CalculatedRequiredLevyAmount calculatedRequiredLevyAmount;
        private long ukprn;
        private LevyAccountModel levyAccountModel;
        private LevyAccountModel mappedLevyAccountModel;
        private long accountId;
        private List<FundingSourcePaymentEvent> fundingSourcePaymentEvents;

        [SetUp]
        public void SetUp()
        {
            ukprn = 3383742;
            accountId = 78934234;
            message = new ProcessUnableToFundTransferFundingSourcePayment
            {
                AccountId = accountId,
                Ukprn = ukprn
            };

            calculatedRequiredLevyAmount = new CalculatedRequiredLevyAmount
            {
                Ukprn = ukprn
            };

            levyAccountModel = new LevyAccountModel
            {
                AccountId = accountId,
                Balance = 3000,
                TransferAllowance = 2000
            };

            mappedLevyAccountModel = new LevyAccountModel
            {
                AccountId = accountId,
                Balance = 3000,
                TransferAllowance = 2000
            };

            fundingSourcePaymentEvents = new List<FundingSourcePaymentEvent>
            {
                new LevyFundingSourcePaymentEvent
                {
                    AccountId = accountId,
                    Ukprn = ukprn,
                    AmountDue = 1000
                }
            };

            paymentLogger = new Mock<IPaymentLogger>();
            mapper = new Mock<IMapper>();
            monthEndCache = new Mock<IDataCache<bool>>();
            levyAccountCache = new Mock<IDataCache<LevyAccountModel>>();
            levyBalanceService = new Mock<ILevyBalanceService>();
            fundingSourcePaymentEventBuilder = new Mock<IFundingSourcePaymentEventBuilder>();
            levyTransactionBatchStorageService = new Mock<ILevyTransactionBatchStorageService>();

            mapper.Setup(x => x.Map<CalculatedRequiredLevyAmount>(message))
                .Returns(calculatedRequiredLevyAmount);

            mapper.Setup(x => x.Map<LevyAccountModel>(levyAccountModel))
                .Returns(mappedLevyAccountModel);

            levyAccountCache.Setup(x => x.TryGet(CacheKeys.LevyBalanceKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<LevyAccountModel>(true, levyAccountModel));

            fundingSourcePaymentEventBuilder.Setup(x => x.BuildFundingSourcePaymentsForRequiredPayment(
                    It.IsAny<CalculatedRequiredLevyAmount>(),
                    accountId,
                    It.IsAny<long>()))
                .Returns(fundingSourcePaymentEvents);

            service = new TransferFundingSourceEventGenerationService(
                paymentLogger.Object,
                mapper.Object,
                monthEndCache.Object,
                levyAccountCache.Object,
                levyBalanceService.Object,
                fundingSourcePaymentEventBuilder.Object,
                levyTransactionBatchStorageService.Object
            );
        }

        [Test]
        public async Task ProcessReceiverTransferPayment_ShouldStoreLevyTransactionsCorrectly()
        {
            await service.ProcessReceiverTransferPayment(message);

            levyTransactionBatchStorageService.Verify(storageService => storageService.StoreLevyTransactions(It.Is<List<CalculatedRequiredLevyAmount>>(levyAmounts =>
                levyAmounts.Any(levyAmount => levyAmount == calculatedRequiredLevyAmount)
                && levyAmounts.Count == 1
            ), It.IsAny<CancellationToken>()));
        }

        [Test]
        public void ProcessReceiverTransferPayment_ShouldValidateAccountId()
        {
            message.AccountId = null;

            Assert.ThrowsAsync<InvalidOperationException>(() => service.ProcessReceiverTransferPayment(message));
        }

        [Test]
        public async Task ProcessReceiverTransferPayment_WhenMonthEndStarted_ShouldQueryLevyAccountCache()
        {
            monthEndCache.Setup(x => x.TryGet(CacheKeys.MonthEndCacheKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<bool>(true, true));

            await service.ProcessReceiverTransferPayment(message);

            levyAccountCache.Verify(x => x.TryGet(CacheKeys.LevyBalanceKey, It.IsAny<CancellationToken>()));
        }

        [Test]
        public void ProcessReceiverTransferPayment_WhenMonthEndStarted_ShouldValidateLevyAccountCacheItem()
        {
            monthEndCache.Setup(x => x.TryGet(CacheKeys.MonthEndCacheKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<bool>(true, true));

            levyAccountCache.Setup(x => x.TryGet(CacheKeys.LevyBalanceKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<LevyAccountModel>(false, null));

            Assert.ThrowsAsync<InvalidOperationException>(() => service.ProcessReceiverTransferPayment(message));
        }

        [Test]
        public async Task ProcessReceiverTransferPayment_WhenMonthEndStarted_ShouldInitialiseLevyBalanceService()
        {
            monthEndCache.Setup(x => x.TryGet(CacheKeys.MonthEndCacheKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<bool>(true, true));

            await service.ProcessReceiverTransferPayment(message);

            levyBalanceService.Verify(x => x.Initialise(levyAccountModel.Balance, levyAccountModel.TransferAllowance));
        }

        [Test]
        public async Task ProcessReceiverTransferPayment_WhenMonthEndStarted_ShouldMapLevyAccountModel()
        {
            monthEndCache.Setup(x => x.TryGet(CacheKeys.MonthEndCacheKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<bool>(true, true));

            await service.ProcessReceiverTransferPayment(message);

            mapper.Verify(x => x.Map<LevyAccountModel>(levyAccountModel));
        }

        [Test]
        public async Task ProcessReceiverTransferPayment_WhenMonthEndStarted_ShouldUpdateLevyAccountCache()
        {
            monthEndCache.Setup(x => x.TryGet(CacheKeys.MonthEndCacheKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<bool>(true, true));

            await service.ProcessReceiverTransferPayment(message);

            levyAccountCache.Verify(x => x.AddOrReplace(CacheKeys.LevyBalanceKey, mappedLevyAccountModel, It.IsAny<CancellationToken>()));
        }
    }
}