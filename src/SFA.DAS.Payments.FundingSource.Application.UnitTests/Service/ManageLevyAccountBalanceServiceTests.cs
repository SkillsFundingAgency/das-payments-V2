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
        private Mock<ILevyAccountBulkCopyRepository<LevyAccountModel>> bulkWriter;

        [SetUp]
        public void Setup()
        {
            repository = new Mock<ILevyFundingSourceRepository>(MockBehavior.Strict);
            accountApiClient = new Mock<IAccountApiClient>(MockBehavior.Strict);
            logger = Mock.Of<IPaymentLogger>();
            bulkWriter = new Mock<ILevyAccountBulkCopyRepository<LevyAccountModel>>(MockBehavior.Strict);
        }


        [Test]
        public async Task Refresh_All_Levy_Account_Details_Correctly()
        {
            var accountIds = new List<long> { 1, 2, 3 };
            int batchSize = 2;
            var accountDetails = new List<AccountDetailViewModel>
            {
                new AccountDetailViewModel
                {
                    AccountId = 1,
                    Balance = 100m,
                    RemainingTransferAllowance = 10m,
                    DasAccountName = "Test Ltd"
                },
                new AccountDetailViewModel
                {
                    AccountId = 2,
                    Balance = 200m,
                    RemainingTransferAllowance = 20m,
                    DasAccountName = "Test 2 Ltd"
                },
                new AccountDetailViewModel
                {
                    AccountId = 3,
                    Balance = 300m,
                    RemainingTransferAllowance = 30m,
                    DasAccountName = "Test 3 Ltd",
                }
            };

            repository
                .Setup(x => x.GetAccountIds(It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountIds);

            accountApiClient
                .SetupSequence(x => x.GetAccount(It.IsAny<long>()))
                .ReturnsAsync(accountDetails[0])
                .ReturnsAsync(accountDetails[1])
                .ReturnsAsync(accountDetails[2]);
            
            repository
                .Setup(x => x.GetNonLevyPayersAccountIds(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<long> {3})
                .Verifiable();
            
            bulkWriter
                .Setup(x => x.Write(It.IsAny<LevyAccountModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            bulkWriter
                .Setup(x => x.Flush(It.IsAny<CancellationToken>()))
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

            repository
                .Verify(x => x.GetNonLevyPayersAccountIds(It.IsAny<CancellationToken>()), Times.Once);

            repository
                .Verify(x => x.GetAccountIds(It.IsAny<CancellationToken>()), Times.Once);

            accountApiClient
                .Verify(x => x.GetAccount(It.IsAny<long>()), Times.Exactly(3));
            
            bulkWriter
                .Verify(x => x.Write(It.IsAny<LevyAccountModel>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
            
            bulkWriter
                .Verify(x => x.Flush(It.IsAny<CancellationToken>()), Times.Exactly(2));
            
        }
        
        [Test]
        public async Task Update_Levy_Account_Details_Correctly()
        {
            var accountIds = new List<long> { 1 };
            int batchSize = 2;
            var accountDetails = new List<AccountDetailViewModel>
            {
                new AccountDetailViewModel
                {
                    AccountId = 1,
                    Balance = 100m,
                    RemainingTransferAllowance = 10m,
                    DasAccountName = "Test 1 Ltd"
                }
            };

            repository
                .Setup(x => x.GetAccountIds(It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountIds);

            accountApiClient
                .SetupSequence(x => x.GetAccount(It.IsAny<long>()))
                .ReturnsAsync(accountDetails[0]);
            
            repository
                .Setup(x => x.GetNonLevyPayersAccountIds(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<long> { 1 })
                .Verifiable();

            bulkWriter
                .Setup(x => x.Write(It.IsAny<LevyAccountModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            bulkWriter
                .Setup(x => x.Flush(It.IsAny<CancellationToken>()))
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

            repository
                .Verify(x => x.GetNonLevyPayersAccountIds(It.IsAny<CancellationToken>()), Times.Once);

            repository
                .Verify(x => x.GetAccountIds(It.IsAny<CancellationToken>()), Times.Once);

            accountApiClient
                .Verify(x => x.GetAccount(accountIds[0]), Times.Once);

            bulkWriter
                .Verify(o => o.Write(It.Is<LevyAccountModel>(x => x.AccountId == accountDetails[0].AccountId &&
                                                                  x.IsLevyPayer == false &&
                                                                  x.AccountName == accountDetails[0].DasAccountName &&
                                                                  x.Balance == accountDetails[0].Balance &&
                                                                  x.TransferAllowance  == accountDetails[0].RemainingTransferAllowance),
                    It.IsAny<CancellationToken>()),
                    Times.Once);

            bulkWriter
                .Verify(x => x.Flush(It.IsAny<CancellationToken>()), Times.Once);

        }

        [Test]
        public async Task Update_IsLevyPayer_Flag_Correctly()
        {
            var accountIds = new List<long> { 1 };
            int batchSize = 2;
            var accountDetails = new List<AccountDetailViewModel>
            {
                new AccountDetailViewModel
                {
                    AccountId = 1,
                    Balance = 100m,
                    RemainingTransferAllowance = 10m,
                    DasAccountName = "Test 1 Ltd"
                }
            };

            repository
                .Setup(x => x.GetNonLevyPayersAccountIds(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<long>());

            repository
                .Setup(x => x.GetAccountIds(It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountIds);

            accountApiClient
                .SetupSequence(x => x.GetAccount(It.IsAny<long>()))
                .ReturnsAsync(accountDetails[0]);

            bulkWriter
                .Setup(x => x.Write(It.IsAny<LevyAccountModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            bulkWriter
                .Setup(x => x.Flush(It.IsAny<CancellationToken>()))
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

            bulkWriter
                .Verify(o => o.Write(It.Is<LevyAccountModel>(x => x.AccountId == accountDetails[0].AccountId &&
                                                                  x.IsLevyPayer == true &&
                                                                  x.AccountName == accountDetails[0].DasAccountName &&
                                                                  x.Balance == accountDetails[0].Balance &&
                                                                  x.TransferAllowance == accountDetails[0].RemainingTransferAllowance),
                    It.IsAny<CancellationToken>()),
                    Times.Once);


        }
    }
}
