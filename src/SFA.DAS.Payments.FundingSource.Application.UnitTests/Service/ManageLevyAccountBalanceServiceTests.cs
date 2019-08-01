using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class ManageLevyAccountBalanceServiceTests
    {
        private Mock<ILevyFundingSourceRepository> repository;
        private Mock<IAccountApiClient> accountApiClient;
        private IPaymentLogger logger;
        private Mock<ILevyAccountBulkCopyRepository> bulkWriter;

        [SetUp]
        public void Setup()
        {
            repository = new Mock<ILevyFundingSourceRepository>(MockBehavior.Strict);
            accountApiClient = new Mock<IAccountApiClient>(MockBehavior.Strict);
            logger = Mock.Of<IPaymentLogger>();
            bulkWriter = new Mock<ILevyAccountBulkCopyRepository>(MockBehavior.Strict);
        }


        [Test]
        public async Task Refresh_All_Levy_Account_Details_Correctly()
        {
            var accountIds = new List<long> { 1, 2, 3 };
            int batchSize = 2;

            var pagedOneApiResponseViewModel = new PagedApiResponseViewModel<AccountWithBalanceViewModel>
            {
                TotalPages = 2,
                Data = new List<AccountWithBalanceViewModel>
                {
                    new AccountWithBalanceViewModel
                    {
                        AccountId = 1,
                        Balance = 100m,
                        RemainingTransferAllowance = 10m,
                        AccountName = "Test Ltd",
                        IsLevyPayer = true
                    }
                }
            };

            var pagedTwoApiResponseViewModel = new PagedApiResponseViewModel<AccountWithBalanceViewModel>
            {
                TotalPages = 2,
                Data = new List<AccountWithBalanceViewModel>
                {
                    new AccountWithBalanceViewModel
                    {
                        AccountId = 2,
                        Balance = 200m,
                        RemainingTransferAllowance = 20m,
                        AccountName = "Test 2 Ltd",
                        IsLevyPayer = true
                    },
                    new AccountWithBalanceViewModel
                    {
                        AccountId = 3,
                        Balance = 300m,
                        RemainingTransferAllowance = 30m,
                        AccountName = "Test 3 Ltd",
                        IsLevyPayer = false
                    }
                }
            };


            accountApiClient
                .SetupSequence(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(pagedOneApiResponseViewModel)
                .ReturnsAsync(pagedTwoApiResponseViewModel);

            bulkWriter
                .Setup(x => x.Write(It.IsAny<LevyAccountModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            bulkWriter
                .Setup(x => x.DeleteAndFlush(It.IsAny<List<long>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);


            var service = new ManageLevyAccountBalanceService
            (
                repository.Object,
                accountApiClient.Object,
                logger,
                bulkWriter.Object,
                batchSize
            );

            await service.RefreshLevyAccountDetails(new CancellationToken()).ConfigureAwait(false);

            accountApiClient
                .Verify(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>()), Times.Exactly(2));

            bulkWriter
                .Verify(x => x.Write(It.IsAny<LevyAccountModel>(), It.IsAny<CancellationToken>()), Times.Exactly(3));

            bulkWriter
                .Verify(x => x.DeleteAndFlush(It.IsAny<List<long>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

        }

        [Test]
        public async Task Update_Levy_Account_Details_Correctly()
        {
            var accountIds = new List<long> { 1 };
            int batchSize = 2;
            var pagedApiResponseViewModel = new PagedApiResponseViewModel<AccountWithBalanceViewModel>
            {
                TotalPages = 1,
                Data = new List<AccountWithBalanceViewModel>
                {
                    new AccountWithBalanceViewModel
                    {
                        AccountId = 1,
                        Balance = 100m,
                        RemainingTransferAllowance = 10m,
                        AccountName = "Test 1 Ltd",
                        IsLevyPayer =  true
                    }
                }
            };

            accountApiClient
                .SetupSequence(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(pagedApiResponseViewModel);

            bulkWriter
                .Setup(x => x.Write(It.IsAny<LevyAccountModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            bulkWriter
                .Setup(x => x.DeleteAndFlush(It.IsAny<List<long>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new ManageLevyAccountBalanceService
            (
                repository.Object,
                accountApiClient.Object,
                logger,
                bulkWriter.Object,
                batchSize
            );

            await service.RefreshLevyAccountDetails(new CancellationToken()).ConfigureAwait(false);

            accountApiClient
                .Verify(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>()), Times.Once);

            bulkWriter
                .Verify(o => o.Write(It.Is<LevyAccountModel>(x => x.AccountId == pagedApiResponseViewModel.Data[0].AccountId &&
                                                                  x.IsLevyPayer == pagedApiResponseViewModel.Data[0].IsLevyPayer &&
                                                                  x.AccountName == pagedApiResponseViewModel.Data[0].AccountName &&
                                                                  x.Balance == pagedApiResponseViewModel.Data[0].Balance &&
                                                                  x.TransferAllowance == pagedApiResponseViewModel.Data[0].RemainingTransferAllowance),
                    It.IsAny<CancellationToken>()),
                    Times.Once);

            bulkWriter
                .Verify(x => x.DeleteAndFlush(It.IsAny<List<long>>(), It.IsAny<CancellationToken>()), Times.Once);

        }


    }
}
