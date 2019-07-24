using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Integrations.Services;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Integrations.UnitTests
{
    [TestFixture]
    public class ManageLevyAccountBalanceServiceTests
    {
        private  Mock<ILevyFundingSourceIntegrationRepository> repository;
        private Mock<IAccountApiClient> accountApiClient;
        private IPaymentLogger logger;
        private Mock<IConfigurationHelper> configurationHelper;

        [SetUp]
        public void Setup()
        {
            repository = new Mock<ILevyFundingSourceIntegrationRepository>(MockBehavior.Strict);
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
                    HashedAccountId = "1xxx",
                    Balance = 100m,
                    RemainingTransferAllowance = 50m,
                    DasAccountName = "Test Ltd"
                },
                new AccountDetailViewModel
                {
                    AccountId = 2,
                    Balance = 200m,
                    HashedAccountId = "2xxx",
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
                .Setup(x => x.AddLevyAccounts(It.IsAny<List<LevyAccountModel>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            repository
                .Setup(x => x.UpdateLevyAccounts(It.IsAny<List<LevyAccountModel>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
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
