using System;
using System.Collections.Generic;
using System.Linq;
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
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
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
        private Mock<IEndpointInstanceFactory> endpointInstanceFactory;
        private Mock<IEndpointInstance> endpointInstance;
        private Mock<ILevyFundingSourceRepository> levyFundingSourceRepository;

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

            bulkWriter
                .Setup(x => x.Write(It.IsAny<LevyAccountModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            bulkWriter
                .Setup(x => x.DeleteAndFlush( It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            levyFundingSourceRepository = new Mock<ILevyFundingSourceRepository>();
        }


        [Test]
        public async Task Refresh_All_Levy_Account_Details_Correctly()
        {
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
           

            var service = CreateManageLevyAccountBalanceService(batchSize);

            await service.RefreshLevyAccountDetails(pageNumber).ConfigureAwait(false);

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
           
            var service = CreateManageLevyAccountBalanceService(batchSize);
            await service.RefreshLevyAccountDetails(1).ConfigureAwait(false);

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
        public async Task Publish_FoundEmployerAccountEvents_Correctly()
        {
            int batchSize = 5;
            List<AccountWithBalanceViewModel> accounts = new List<AccountWithBalanceViewModel>
            {
                new AccountWithBalanceViewModel
                {
                    AccountId = 1,
                    Balance = 100m,
                    RemainingTransferAllowance = 10m,
                    AccountName = "Test Ltd",
                    IsLevyPayer = false
                },
                new AccountWithBalanceViewModel
                {
                    AccountId = 2,
                    Balance = 100m,
                    RemainingTransferAllowance = 10m,
                    AccountName = "Test Ltd",
                    IsLevyPayer = true
                },
                new AccountWithBalanceViewModel
                {
                    AccountId = 3,
                    Balance = 100m,
                    RemainingTransferAllowance = 10m,
                    AccountName = "Test Ltd",
                    IsLevyPayer = true
                }
            };
            var pagedOneApiResponseViewModel = new PagedApiResponseViewModel<AccountWithBalanceViewModel>
            {
                TotalPages = 1,
                Data = accounts
            };

            var savedStatuses = accounts.Select(x => (x.AccountId, !x.IsLevyPayer)); //opposite state to new state
            levyFundingSourceRepository
                .Setup(x => x.GetCurrentEmployerStatus(It.IsAny<List<long>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(savedStatuses.ToList());


            accountApiClient
                .Setup(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(pagedOneApiResponseViewModel);


            var service = CreateManageLevyAccountBalanceService(batchSize);

            await service.RefreshLevyAccountDetails(1, CancellationToken.None).ConfigureAwait(false);

            endpointInstance
                .Verify(svc => svc.Publish(It.Is<FoundNotLevyPayerEmployerAccount>(x => x.AccountId == 1),
                        It.IsAny<PublishOptions>()),
                    Times.Once);
            endpointInstance
                .Verify(svc =>
                        svc.Publish(
                            It.Is<FoundLevyPayerEmployerAccount>(x => new List<long> {2, 3}.Contains(x.AccountId)),
                            It.IsAny<PublishOptions>()),
                    Times.Exactly(2));
        }


        [Test]
        public async Task Publish_FoundEmployerAccountEventsOnlyPublishedMessageIfLevyFlagHasChangedFromRecordedState_Correctly()
        {
            int batchSize = 5;
            List<AccountWithBalanceViewModel> accounts = new List<AccountWithBalanceViewModel>
            {
                new AccountWithBalanceViewModel
                {
                    AccountId = 1,
                    Balance = 100m,
                    RemainingTransferAllowance = 10m,
                    AccountName = "Test Ltd",
                    IsLevyPayer = false
                },
                new AccountWithBalanceViewModel
                {
                    AccountId = 2,
                    Balance = 100m,
                    RemainingTransferAllowance = 10m,
                    AccountName = "Test Ltd",
                    IsLevyPayer = true
                },
                new AccountWithBalanceViewModel
                {
                    AccountId = 3,
                    Balance = 100m,
                    RemainingTransferAllowance = 10m,
                    AccountName = "Test Ltd",
                    IsLevyPayer = true
                }
            };
            var pagedOneApiResponseViewModel = new PagedApiResponseViewModel<AccountWithBalanceViewModel>
            {
                TotalPages = 1,
                Data = accounts
            };

            var currentDbStatuses = new List<(long, bool)>(){ (1, true), (2, false), (3, true) }; // only first two stored states differ from new accounts

            levyFundingSourceRepository.Setup(x=>x.GetCurrentEmployerStatus(It.IsAny<List<long>>(), It.IsAny<CancellationToken>())).ReturnsAsync(currentDbStatuses.ToList());
            
            accountApiClient
                .Setup(x => x.GetPageOfAccounts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(pagedOneApiResponseViewModel);
            
            var service = CreateManageLevyAccountBalanceService(batchSize);

            await service.RefreshLevyAccountDetails(1, CancellationToken.None).ConfigureAwait(false);

            endpointInstance
                .Verify(svc => svc.Publish(It.Is<FoundNotLevyPayerEmployerAccount>( x => x.AccountId == 1),
                    It.IsAny<PublishOptions>()), 
                    Times.Once);
            endpointInstance
                .Verify(svc => svc.Publish(It.Is<FoundLevyPayerEmployerAccount>(x => x.AccountId == 2),
                        It.IsAny<PublishOptions>()),
                    Times.Once);
        }

        private ManageLevyAccountBalanceService CreateManageLevyAccountBalanceService(int batchSize)
        {
            var service = new ManageLevyAccountBalanceService
            (
                accountApiClient.Object,
                logger,
                bulkWriter.Object,
                levyFundingSourceRepository.Object,

                batchSize,
                endpointInstanceFactory.Object
            );
            return service;
        }
    }
}
