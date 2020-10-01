using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class ManageLevyAccountBalanceServiceTests
    {
        private Mock<IAccountApiClient> accountApiClient;
        private IPaymentLogger logger;
        private Mock<ILevyAccountBulkCopyRepository> bulkWriter;
        private  Mock<IEndpointInstanceFactory> endpointInstanceFactory;
        private Mock<IEndpointInstance> endpointInstance;

        [SetUp]
        public void Setup()
        {
            accountApiClient = new Mock<IAccountApiClient>(MockBehavior.Strict);
            logger = Mock.Of<IPaymentLogger>();
            bulkWriter = new Mock<ILevyAccountBulkCopyRepository>(MockBehavior.Strict);
            endpointInstanceFactory = new Mock<IEndpointInstanceFactory>(MockBehavior.Strict);
            endpointInstance = new Mock<IEndpointInstance>(MockBehavior.Strict);

            endpointInstance
                 .Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()))
                .Returns(Task.CompletedTask);
            
            endpointInstanceFactory
                .Setup(x => x.GetEndpointInstance())
                .ReturnsAsync(endpointInstance.Object);
        }


        [Test]
        public async Task Refresh_All_Levy_Account_Details_Correctly()
        {
            var accountIds = new List<long> { 1, 2, 3 };
            int batchSize = 2;
            var pageNumber = 3;

            var apiResponseViewModel = new PagedApiResponseViewModel<AccountWithBalanceViewModel>
            {
                TotalPages = 5,
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

            accountApiClient
                .Setup(x => x.GetPageOfAccounts(pageNumber, It.IsAny<int>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(apiResponseViewModel);
                
            bulkWriter
                .Setup(x => x.Write(It.IsAny<LevyAccountModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            bulkWriter
                .Setup(x => x.DeleteAndFlush( It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);


            var service = new ManageLevyAccountBalanceService
            (
                accountApiClient.Object,
                logger,
                bulkWriter.Object,
                batchSize,
                endpointInstanceFactory.Object
            );

            await service.RefreshLevyAccountDetails(pageNumber, new CancellationToken()).ConfigureAwait(false);

            accountApiClient
                .Verify(x => x.GetPageOfAccounts(pageNumber, It.IsAny<int>(), It.IsAny<DateTime?>()), Times.Exactly(1));

            bulkWriter
                .Verify(x => x.Write(It.IsAny<LevyAccountModel>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

            bulkWriter
                .Verify(x => x.DeleteAndFlush(It.IsAny<CancellationToken>()), Times.Exactly(1));

        }

        [Test]
        public async Task Update_Levy_Account_Details_Correctly()
        {
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
                .Setup(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(pagedApiResponseViewModel);

            bulkWriter
                .Setup(x => x.Write(It.IsAny<LevyAccountModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            bulkWriter
                .Setup(x => x.DeleteAndFlush( It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new ManageLevyAccountBalanceService
            (
                accountApiClient.Object,
                logger,
                bulkWriter.Object,
                batchSize,
                endpointInstanceFactory.Object
            );

            await service.RefreshLevyAccountDetails(1, new CancellationToken()).ConfigureAwait(false);

            accountApiClient
                .Verify(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>()), Times.Exactly(1));

            bulkWriter
                .Verify(o => o.Write(It.Is<LevyAccountModel>(x => x.AccountId == pagedApiResponseViewModel.Data[0].AccountId &&
                                                                  x.IsLevyPayer == pagedApiResponseViewModel.Data[0].IsLevyPayer &&
                                                                  x.AccountName == pagedApiResponseViewModel.Data[0].AccountName &&
                                                                  x.Balance == pagedApiResponseViewModel.Data[0].Balance &&
                                                                  x.TransferAllowance == pagedApiResponseViewModel.Data[0].RemainingTransferAllowance),
                    It.IsAny<CancellationToken>()),
                    Times.Once);

            bulkWriter
                .Verify(x => x.DeleteAndFlush(It.IsAny<CancellationToken>()), Times.Once);

        }

        [Test]
        public async Task Publish_FoundNotLevyPayerEmployerAccount_Correctly()
        {
            int batchSize = 5;
            var pagedOneApiResponseViewModel = new PagedApiResponseViewModel<AccountWithBalanceViewModel>
            {
                TotalPages = 1,
                Data = new List<AccountWithBalanceViewModel>
                {
                    new AccountWithBalanceViewModel
                    {
                        AccountId = 1,
                        Balance = 100m,
                        RemainingTransferAllowance = 10m,
                        AccountName = "Test Ltd",
                        IsLevyPayer = false
                    }
                }
            };

            accountApiClient
                .Setup(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(pagedOneApiResponseViewModel);
            
            bulkWriter
                .Setup(x => x.Write(It.IsAny<LevyAccountModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            bulkWriter
                .Setup(x => x.DeleteAndFlush(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);


            var service = new ManageLevyAccountBalanceService
            (
                accountApiClient.Object,
                logger,
                bulkWriter.Object,
                batchSize,
                endpointInstanceFactory.Object
            );

            await service.RefreshLevyAccountDetails(1, CancellationToken.None).ConfigureAwait(false);

            endpointInstance
                .Verify(svc => svc.Publish(It.Is<FoundNotLevyPayerEmployerAccount>( x => x.AccountId == 1),
                    It.IsAny<PublishOptions>()), 
                    Times.Once);

        }


        [Test]
        public async Task FoundNotLevyPayerEmployerAccount_Event_Is_Not_Published_If_All_Employer_Is_Marked_As_IsLevyPayer()
        {
            int batchSize = 5;
            var pagedOneApiResponseViewModel = new PagedApiResponseViewModel<AccountWithBalanceViewModel>
            {
                TotalPages = 1,
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

            accountApiClient
                .Setup(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(pagedOneApiResponseViewModel);

            bulkWriter
                .Setup(x => x.Write(It.IsAny<LevyAccountModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            bulkWriter
                .Setup(x => x.DeleteAndFlush(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);


            var service = new ManageLevyAccountBalanceService
            (
                accountApiClient.Object,
                logger,
                bulkWriter.Object,
                batchSize,
                endpointInstanceFactory.Object
            );

            await service.RefreshLevyAccountDetails(1, CancellationToken.None).ConfigureAwait(false);

            endpointInstance
                .Verify(svc => svc.Publish(It.Is<FoundNotLevyPayerEmployerAccount>(x => x.AccountId == 1),
                    It.IsAny<PublishOptions>()),
                    Times.Never);

        }
    }
}
