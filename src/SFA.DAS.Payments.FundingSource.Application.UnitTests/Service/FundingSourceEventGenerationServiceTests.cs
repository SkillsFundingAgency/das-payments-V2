using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class FundingSourceEventGenerationServiceTests
    {
        private Mock<IPaymentLogger> logger;
        private Mock<IFundingSourceDataContext> dataContext;
        private Mock<ILevyBalanceService> levyBalanceService;
        private Mock<ILevyFundingSourceRepository> levyFundingSourceRepository;
        private Mock<IDataCache<LevyAccountModel>> levyAccountCache;
        private Mock<ICalculatedRequiredLevyAmountPrioritisationService> calculatedRequiredLevyAmountPrioritisationService;
        private Mock<IFundingSourcePaymentEventBuilder> fundingSourcePaymentEventBuilder;

        private FundingSourceEventGenerationService service;

        private long employerAccountId;
        private long jobId;
        private LevyAccountModel levyAccount;
        private List<EmployerProviderPriorityModel> priorities;
        private List<LevyTransactionModel> levyTransactions;
        private List<CalculatedRequiredLevyAmount> prioritisedTransactions;
        private List<FundingSourcePaymentEvent> firstPriorityFundingSourcePaymentEvents;
        private List<FundingSourcePaymentEvent> secondPriorityFundingSourcePaymentEvents;
        private decimal initialLevyBalance;
        private decimal initialTransferAllowance;
        private decimal firstPriorityCalculatedRequiredLevyAmountAmountDue;
        private decimal secondPriorityCalculatedRequiredLevyAmountAmountDue;
        private decimal remainingBalance;
        private decimal remainingTransferAllowance;
        private long firstPriorityUkprn;
        private long secondPriorityUkprn;

        [SetUp]
        public void SetUp()
        {
            employerAccountId = 112;
            jobId = 114;
            initialLevyBalance = 2000;
            initialTransferAllowance = 1000;
            remainingBalance = 900;
            remainingTransferAllowance = 800;

            levyAccount = new LevyAccountModel
            {
                Balance = initialLevyBalance,
                TransferAllowance = initialTransferAllowance
            };

            firstPriorityCalculatedRequiredLevyAmountAmountDue = 3000;
            firstPriorityUkprn = 668498390;
            secondPriorityCalculatedRequiredLevyAmountAmountDue = 1500;
            secondPriorityUkprn = 228733629;

            priorities = new List<EmployerProviderPriorityModel>
            {
                new EmployerProviderPriorityModel{ EmployerAccountId = employerAccountId, Id = 116, Order = 2, Ukprn = secondPriorityUkprn },
                new EmployerProviderPriorityModel{ EmployerAccountId = employerAccountId, Id = 116, Order = 1, Ukprn = firstPriorityUkprn }
            };

            levyTransactions = new List<LevyTransactionModel>
            {
                new LevyTransactionModel{ AccountId = employerAccountId, Amount = 1500, MessagePayload = JsonConvert.SerializeObject(new CalculatedRequiredLevyAmount
                {
                    AccountId = employerAccountId,
                    AmountDue = secondPriorityCalculatedRequiredLevyAmountAmountDue,
                    Ukprn = secondPriorityUkprn
                })},
                new LevyTransactionModel{ AccountId = employerAccountId, Amount = 3000, MessagePayload = JsonConvert.SerializeObject(new CalculatedRequiredLevyAmount
                {
                    AccountId = employerAccountId,
                    AmountDue = firstPriorityCalculatedRequiredLevyAmountAmountDue,
                    Ukprn = firstPriorityUkprn
                })}
            };

            prioritisedTransactions = new List<CalculatedRequiredLevyAmount>
            {
                new CalculatedRequiredLevyAmount
                {
                    AccountId = employerAccountId,
                    AmountDue = firstPriorityCalculatedRequiredLevyAmountAmountDue,
                    Ukprn = firstPriorityUkprn
                },
                new CalculatedRequiredLevyAmount
                {
                    AccountId = employerAccountId,
                    AmountDue = secondPriorityCalculatedRequiredLevyAmountAmountDue,
                    Ukprn = secondPriorityUkprn
                }
            };

            firstPriorityFundingSourcePaymentEvents = new List<FundingSourcePaymentEvent>
            {
                new EmployerCoInvestedFundingSourcePaymentEvent{ AmountDue = 540 },
                new LevyFundingSourcePaymentEvent{ AmountDue = 700 },
                new TransferFundingSourcePaymentEvent{ AmountDue = 900 }
            };

            secondPriorityFundingSourcePaymentEvents = new List<FundingSourcePaymentEvent>
            {
                new TransferFundingSourcePaymentEvent{ AmountDue = 2500 }
            };

            logger = new Mock<IPaymentLogger>();
            dataContext = new Mock<IFundingSourceDataContext>();
            levyBalanceService = new Mock<ILevyBalanceService>();
            levyFundingSourceRepository = new Mock<ILevyFundingSourceRepository>();
            levyBalanceService = new Mock<ILevyBalanceService>();
            levyAccountCache = new Mock<IDataCache<LevyAccountModel>>();
            calculatedRequiredLevyAmountPrioritisationService = new Mock<ICalculatedRequiredLevyAmountPrioritisationService>();
            fundingSourcePaymentEventBuilder = new Mock<IFundingSourcePaymentEventBuilder>();

            levyFundingSourceRepository.Setup(x => x.GetLevyAccount(employerAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(levyAccount);

            dataContext.Setup(x => x.GetEmployerProviderPriorities(employerAccountId))
                .Returns(priorities.AsQueryable());

            dataContext.Setup(x => x.GetEmployerLevyTransactions(employerAccountId))
                .Returns(levyTransactions.AsQueryable());

            calculatedRequiredLevyAmountPrioritisationService
                .Setup(x => x.Prioritise(It.IsAny<List<CalculatedRequiredLevyAmount>>(), It.IsAny<List<(long Ukprn, int Order)>>()))
                .ReturnsAsync(prioritisedTransactions);

            fundingSourcePaymentEventBuilder
                .Setup(x => x.BuildFundingSourcePaymentsForRequiredPayment(It.Is<CalculatedRequiredLevyAmount>(y => y.Ukprn == firstPriorityUkprn), employerAccountId, jobId))
                .Returns(firstPriorityFundingSourcePaymentEvents);

            fundingSourcePaymentEventBuilder
                .Setup(x => x.BuildFundingSourcePaymentsForRequiredPayment(It.Is<CalculatedRequiredLevyAmount>(y => y.Ukprn == secondPriorityUkprn), employerAccountId, jobId))
                .Returns(secondPriorityFundingSourcePaymentEvents);

            levyBalanceService.SetupGet(x => x.RemainingBalance).Returns(remainingBalance);
            levyBalanceService.SetupGet(x => x.RemainingTransferAllowance).Returns(remainingTransferAllowance);

            service = new FundingSourceEventGenerationService(
                logger.Object,
                dataContext.Object,
                levyBalanceService.Object,
                levyFundingSourceRepository.Object,
                levyAccountCache.Object,
                calculatedRequiredLevyAmountPrioritisationService.Object,
                fundingSourcePaymentEventBuilder.Object);
        }

        [Test]
        public async Task HandleMonthEnd_ShouldGetLevyAccount()
        {
            await service.HandleMonthEnd(employerAccountId, jobId);

            levyFundingSourceRepository.Verify(x => x.GetLevyAccount(employerAccountId, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task HandleMonthEnd_ShouldInitialiseLevyBalanceService()
        {
            await service.HandleMonthEnd(employerAccountId, jobId);

            levyBalanceService.Verify(x => x.Initialise(initialLevyBalance, initialTransferAllowance));
        }

        [Test]
        public async Task HandleMonthEnd_ShouldGetEmployerProviderPriorities()
        {
            await service.HandleMonthEnd(employerAccountId, jobId);

            dataContext.Verify(x => x.GetEmployerProviderPriorities(employerAccountId));
        }

        [Test]
        public async Task HandleMonthEnd_ShouldGetEmployerLevyTransactions()
        {
            await service.HandleMonthEnd(employerAccountId, jobId);

            dataContext.Verify(x => x.GetEmployerLevyTransactions(employerAccountId));
        }

        [Test]
        public async Task HandleMonthEnd_ShouldPrioritise()
        {
            await service.HandleMonthEnd(employerAccountId, jobId);

            calculatedRequiredLevyAmountPrioritisationService.Verify(prioritisationService => prioritisationService.Prioritise(It.Is<List<CalculatedRequiredLevyAmount>>(
                levyAmounts =>
                    levyAmounts.Count == 2
                    && levyAmounts[0].AccountId == employerAccountId 
                    && levyAmounts[0].AmountDue == secondPriorityCalculatedRequiredLevyAmountAmountDue 
                    && levyAmounts[0].Ukprn == secondPriorityUkprn
                    && levyAmounts[1].AccountId == employerAccountId
                    && levyAmounts[1].AmountDue == firstPriorityCalculatedRequiredLevyAmountAmountDue
                    && levyAmounts[1].Ukprn == firstPriorityUkprn
            ), It.Is<List<(long ukprn, int order)>>(
                employerPriorities => 
                    priorities.Count == 2
                    && priorities.Any(priority => priority.Ukprn == priorities[0].Ukprn && priority.Order == priorities[0].Order)
                    && priorities.Any(priority => priority.Ukprn == priorities[1].Ukprn && priority.Order == priorities[1].Order)
            )));
        }

        [Test]
        public async Task HandleMonthEnd_ShouldBuildFundingSourcePayments()
        {
            await service.HandleMonthEnd(employerAccountId, jobId);

            fundingSourcePaymentEventBuilder.Verify(x => x.BuildFundingSourcePaymentsForRequiredPayment(prioritisedTransactions[0], employerAccountId, jobId));
            fundingSourcePaymentEventBuilder.Verify(x => x.BuildFundingSourcePaymentsForRequiredPayment(prioritisedTransactions[1], employerAccountId, jobId));
        }

        [Test]
        public async Task HandleMonthEnd_ShouldUpdateLevyAccountCache()
        {
            await service.HandleMonthEnd(employerAccountId, jobId);

            levyAccountCache
                .Verify(x => x.AddOrReplace(CacheKeys.LevyBalanceKey, It.Is<LevyAccountModel>(model => 
                    model.Balance == remainingBalance 
                    && model.TransferAllowance == remainingTransferAllowance
                ), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task HandleMonthEnd_ShouldReturnExpectedFundingSourcePaymentEvents()
        {
            var result = await service.HandleMonthEnd(employerAccountId, jobId);

            //Should be 4 total
            Assert.That(result.Count == 4);

            //First three should match the builder result from the builder call for the first item returned in the priority list
            Assert.That(result.Any(x =>
                x.AmountDue == firstPriorityFundingSourcePaymentEvents[0].AmountDue
                && x.FundingSourceType == firstPriorityFundingSourcePaymentEvents[0].FundingSourceType));
            Assert.That(result.Any(x =>
                x.AmountDue == firstPriorityFundingSourcePaymentEvents[1].AmountDue
                && x.FundingSourceType == firstPriorityFundingSourcePaymentEvents[1].FundingSourceType));
            Assert.That(result.Any(x =>
                x.AmountDue == firstPriorityFundingSourcePaymentEvents[2].AmountDue
                && x.FundingSourceType == firstPriorityFundingSourcePaymentEvents[2].FundingSourceType));

            //Last one should match the builder result from the builder call for the second item returned in the priority list
            Assert.That(result.Last().AmountDue == secondPriorityFundingSourcePaymentEvents[0].AmountDue);
            Assert.That(result.Last().FundingSourceType == secondPriorityFundingSourcePaymentEvents[0].FundingSourceType);
        }
    }
}