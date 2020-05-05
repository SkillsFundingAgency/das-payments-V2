using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Components.DictionaryAdapter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
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
        private Mock<IMapper> mapper;
        private Mock<IPaymentProcessor> processor;
        private Mock<ILevyFundingSourceRepository> levyFundingSourceRepository;
        private Mock<IDataCache<LevyAccountModel>> levyAccountCache;
        private Mock<ICalculatedRequiredLevyAmountPrioritisationService> calculatedRequiredLevyAmountPrioritisationService;

        private FundingSourceEventGenerationService service;

        private long employerAccountId;
        private long jobId;
        private LevyAccountModel levyAccount;
        private List<EmployerProviderPriorityModel> priorities;
        private List<LevyTransactionModel> levyTransactions;
        private List<CalculatedRequiredLevyAmount> prioritisedTransactions;
        private List<FundingSourcePayment> fundingSourcePayments;

        [SetUp]
        public void SetUp()
        {
            employerAccountId = 112;
            jobId = 114;
            levyAccount = new LevyAccountModel
            {
                Balance = 2000,
                TransferAllowance = 1000
            };

            priorities = new List<EmployerProviderPriorityModel>
            {
                new EmployerProviderPriorityModel{ EmployerAccountId = employerAccountId, Id = 116, Order = 1, Ukprn = 228733629 },
                new EmployerProviderPriorityModel{ EmployerAccountId = employerAccountId, Id = 116, Order = 2, Ukprn = 668498390 }
            };

            levyTransactions = new List<LevyTransactionModel>
            {
                new LevyTransactionModel{ AccountId = employerAccountId, Amount = 1500, MessagePayload = JsonConvert.SerializeObject(new CalculatedRequiredLevyAmount
                {
                    AccountId = employerAccountId,
                    AmountDue = 1500,
                    AgreementId = "Agreement One"
                })},
                new LevyTransactionModel{ AccountId = employerAccountId, Amount = 3000, MessagePayload = JsonConvert.SerializeObject(new CalculatedRequiredLevyAmount
                {
                    AccountId = employerAccountId,
                    AmountDue = 3000,
                    AgreementId = "Agreement Two"
                })}
            };

            prioritisedTransactions = new List<CalculatedRequiredLevyAmount>
            {
                new CalculatedRequiredLevyAmount
                {
                    AccountId = employerAccountId,
                    AmountDue = 3000,
                    AgreementId = "Agreement Two"
                },
                new CalculatedRequiredLevyAmount
                {
                    AccountId = employerAccountId,
                    AmountDue = 1500,
                    AgreementId = "Agreement One"
                }
            };

            fundingSourcePayments = new List<FundingSourcePayment>
            {
                new EmployerCoInvestedPayment{ AmountDue = 540, Type = FundingSourceType.CoInvestedSfa },
                new LevyPayment{ AmountDue = 700, Type = FundingSourceType.Levy },
                new TransferPayment{ AmountDue = 900, Type = FundingSourceType.Transfer }
            };

            logger = new Mock<IPaymentLogger>();
            dataContext = new Mock<IFundingSourceDataContext>();
            levyBalanceService = new Mock<ILevyBalanceService>();
            mapper = new Mock<IMapper>();
            processor = new Mock<IPaymentProcessor>();
            levyFundingSourceRepository = new Mock<ILevyFundingSourceRepository>();
            levyBalanceService = new Mock<ILevyBalanceService>();
            levyAccountCache = new Mock<IDataCache<LevyAccountModel>>();
            calculatedRequiredLevyAmountPrioritisationService = new Mock<ICalculatedRequiredLevyAmountPrioritisationService>();

            levyFundingSourceRepository.Setup(x => x.GetLevyAccount(employerAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(levyAccount);

            dataContext.Setup(x => x.GetEmployerProviderPriorities(employerAccountId))
                .Returns(priorities.AsQueryable());

            dataContext.Setup(x => x.GetEmployerLevyTransactions(employerAccountId))
                .Returns(levyTransactions.AsQueryable());

            calculatedRequiredLevyAmountPrioritisationService
                .Setup(x => x.Prioritise(It.IsAny<List<CalculatedRequiredLevyAmount>>(), It.IsAny<List<(long Ukprn, int Order)>>()))
                .ReturnsAsync(prioritisedTransactions);

            processor.Setup(x => x.Process(It.IsAny<RequiredPayment>()))
                .Returns(fundingSourcePayments);

            service = new FundingSourceEventGenerationService(
                logger.Object,
                dataContext.Object,
                levyBalanceService.Object,
                mapper.Object,
                processor.Object,
                levyFundingSourceRepository.Object,
                levyAccountCache.Object,
                calculatedRequiredLevyAmountPrioritisationService.Object);
        }

        [Test]
        public async Task HandleMonthEnd_ShouldGetLevyAccount()
        {
            await service.HandleMonthEnd(employerAccountId, jobId);

            levyFundingSourceRepository.Verify(x => x.GetLevyAccount(employerAccountId, It.IsAny<CancellationToken>()));
        }
    }
}