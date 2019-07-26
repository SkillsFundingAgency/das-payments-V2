using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class ManageLevyAccountBalanceServiceTests
    {
        private  Mock<ILevyFundingSourceRepository> repository;
        private Mock<IAccountApiClient> accountApiClient;
        private IPaymentLogger logger;
        private Mock<IConfigurationHelper> configurationHelper;

        [SetUp]
        public void Setup()
        {
            repository = new Mock<ILevyFundingSourceRepository>(MockBehavior.Strict);
            accountApiClient = new Mock<IAccountApiClient>(MockBehavior.Strict);
            logger = Mock.Of<IPaymentLogger>();
            configurationHelper = new Mock<IConfigurationHelper>();
        }

    
        [Test]
        public async Task Refresh_Levy_Account_Details_Correctly()
        {
            var accountIds = new List<long> {1, 2};
            int batchSize = 1;
            var accountDetails = new List<AccountDetailViewModel>
            {
                new AccountDetailViewModel
                {
                    AccountId = 1,
                    Balance = 100m,
                    RemainingTransferAllowance = 50m,
                    DasAccountName = "Test Ltd"
                },
                new AccountDetailViewModel
                {
                    AccountId = 2,
                    Balance = 200m,
                    RemainingTransferAllowance = 20m,
                    DasAccountName = "Test 2 Ltd"
                }
            };

            repository
                .Setup(x => x.GetAccountIds(It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountIds);
            
            accountApiClient
                .SetupSequence(x => x.GetAccount(It.IsAny<long>()))
                .ReturnsAsync(accountDetails[0])
                .ReturnsAsync(accountDetails[1]);

            LevyAccountModel nullLevyAccountModel = null;

            repository
                .SetupSequence(x => x.GetLevyAccount(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LevyAccountModel
                {
                    AccountId = 1,
                    Balance = 400,
                    AccountName = "Old Ltd"
                })
                .ReturnsAsync(nullLevyAccountModel);

            repository
                .Setup(x => x.AddLevyAccounts(
                    It.Is<List<LevyAccountModel>>(o => o[1].AccountId == accountDetails[1].AccountId &&
                                                       o[1].Balance == accountDetails[1].Balance &&
                                                       o[1].TransferAllowance == accountDetails[1].RemainingTransferAllowance ),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            repository
                .Setup(x => x.UpdateLevyAccounts(
                    It.Is<List<LevyAccountModel>>(o => o[0].AccountId == accountDetails[0].AccountId &&
                                                       o[0].Balance == accountDetails[0].Balance &&
                                                       o[0].TransferAllowance == accountDetails[0].RemainingTransferAllowance ),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var service = new ManageLevyAccountBalanceService
            (
                repository.Object,
                accountApiClient.Object,
                logger,
                batchSize
            );

            await service.RefreshLevyAccountDetails(new CancellationToken()).ConfigureAwait(false);
            
            repository
                .Verify(x => x.GetAccountIds(It.IsAny<CancellationToken>()), Times.Once);
            
            accountApiClient
                .Verify(x => x.GetAccount(It.IsAny<long>()), Times.Exactly(2));

            repository
                .Verify(x => x.UpdateLevyAccounts (It.IsAny<List<LevyAccountModel>>(), It.IsAny<CancellationToken>()), Times.Once);
                
            repository
                .Verify(x => x.AddLevyAccounts(It.IsAny<List<LevyAccountModel>>(), It.IsAny<CancellationToken>()), Times.Once);
            
        }
        
    }
}
