using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using NServiceBus;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public interface IManageLevyAccountBalanceService
    {
        Task RefreshLevyAccountDetails(CancellationToken cancellationToken = default(CancellationToken));
    }

    public class ManageLevyAccountBalanceService : IManageLevyAccountBalanceService
    {
        private readonly ILevyFundingSourceRepository repository;
        private readonly IAccountApiClient accountApiClient;
        private readonly IPaymentLogger logger;
        private readonly ILevyAccountBulkCopyRepository levyAccountBulkWriter;
        private readonly int batchSize;
        private int totalPageSize;
        private readonly AsyncRetryPolicy retryPolicy;
        private readonly IEndpointInstanceFactory endpointInstanceFactory;

        public ManageLevyAccountBalanceService(ILevyFundingSourceRepository repository,
            IAccountApiClient accountApiClient,
            IPaymentLogger logger,
            ILevyAccountBulkCopyRepository levyAccountBulkWriter,
            int batchSize,
            IEndpointInstanceFactory endpointInstanceFactory)
        {
            this.repository = repository;
            this.accountApiClient = accountApiClient;
            this.logger = logger;
            this.levyAccountBulkWriter = levyAccountBulkWriter;
            this.batchSize = batchSize;
            this.endpointInstanceFactory = endpointInstanceFactory;

            retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(5, i => TimeSpan.FromMinutes(1));
        }

        public async Task RefreshLevyAccountDetails(CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.LogInfo("Now Trying to Refresh All Accounts Balance Details");

            var page = 1;

            await retryPolicy.ExecuteAsync(GetTotalPageSize).ConfigureAwait(false);

            while (page <= totalPageSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var pagedAccountsRecords = await accountApiClient.GetPageOfAccounts(page, batchSize).ConfigureAwait(false);
                    var pagedLevyAccountModels = MapToLevyAccountModel(pagedAccountsRecords);
                    await BatchUpdateLevyAccounts(pagedLevyAccountModels, cancellationToken).ConfigureAwait(false);
                    await PublishNotLevyPayerEmployerEvents(pagedLevyAccountModels).ConfigureAwait(false);

                    logger.LogInfo($"Successfully retrieved Account Balance Details for Page {page} of Levy Accounts");
                }
                catch (Exception e)
                {
                    logger.LogError($"Error while retrieving Account Balance Details for Page {page} of Levy Accounts", e);
                }

                page++;
            }
        }
        private async Task BatchUpdateLevyAccounts(List<LevyAccountModel> levyAccountModels, CancellationToken cancellationToken)
        {
            try
            {
                await Task.WhenAll(levyAccountModels.Select(x => levyAccountBulkWriter.Write(x, cancellationToken))).ConfigureAwait(false);
                await levyAccountBulkWriter.DeleteAndFlush(cancellationToken).ConfigureAwait(false);
                logger.LogVerbose($"Successfully Added  {levyAccountModels.Count} Batch of Levy Accounts Details");
            }
            catch (Exception e)
            {
                logger.LogError($"Error while Adding  {levyAccountModels.Count} Batch of Levy Accounts Details", e);
            }
        }

        private List<LevyAccountModel> MapToLevyAccountModel(PagedApiResponseViewModel<AccountWithBalanceViewModel> pagedAccountWithBalanceViewModel)
        {
            var levyAccountModels = pagedAccountWithBalanceViewModel.Data.Select(accountDetail => new LevyAccountModel
            {
                AccountId = accountDetail.AccountId,
                IsLevyPayer = accountDetail.IsLevyPayer,
                AccountName = accountDetail.AccountName,
                Balance = accountDetail.Balance,
                TransferAllowance = accountDetail.RemainingTransferAllowance,
            }).ToList();

            return levyAccountModels;

        }

        private async Task GetTotalPageSize()
        {
            try
            {
                var pagedAccountsRecord = await accountApiClient.GetPageOfAccounts(1, batchSize).ConfigureAwait(false);
                totalPageSize = pagedAccountsRecord.TotalPages;

                logger.LogInfo($"Total Levy Account to process {totalPageSize} ");
            }
            catch (Exception e)
            {
                logger.LogError("Error while trying to get Total number of Levy Accounts", e);
                throw;
            }

        }

        private async Task PublishNotLevyPayerEmployerEvents(List<LevyAccountModel> accountModels)
        {
            var notLevyPayingEmployerIds = accountModels.Where(x => !x.IsLevyPayer).Select(x => x.AccountId).ToList();

            if (!notLevyPayingEmployerIds.Any()) return;
            logger.LogInfo($"Trying to Publish FoundNotLevyPayerEmployerAccount event for  Account Ids: {string.Join(",", notLevyPayingEmployerIds)}");

            var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
            var publishMessageTasks = notLevyPayingEmployerIds.Select(accountId => endpointInstance.Publish(new FoundNotLevyPayerEmployerAccount { AccountId = accountId }));
            await Task.WhenAll(publishMessageTasks).ConfigureAwait(false);

            logger.LogInfo($"Successfully Published FoundNotLevyPayerEmployerAccount event for  Account Ids: {string.Join(",", notLevyPayingEmployerIds)}");
        }

    }
}
