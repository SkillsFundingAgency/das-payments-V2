using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
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
        private readonly IBulkWriter<LevyAccountModel> bulkWriter;
        private readonly int batchSize;

        public ManageLevyAccountBalanceService(ILevyFundingSourceRepository repository,
            IAccountApiClient accountApiClient,
            IPaymentLogger logger,
            IBulkWriter<LevyAccountModel> bulkWriter,
            int batchSize)
        {
            this.repository = repository;
            this.accountApiClient = accountApiClient;
            this.logger = logger;
            this.bulkWriter = bulkWriter;
            this.batchSize = batchSize;
        }

        public async Task RefreshLevyAccountDetails(CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.LogInfo($"Now Trying to Refresh All Accounts Balance Details");

            var levyPayerAccountIds = await repository.GetLevyPayerAccountIds(cancellationToken).ConfigureAwait(false);

            var accountIds = await repository.GetAccountIds(cancellationToken).ConfigureAwait(false);

            logger.LogInfo($"Retrieving Account Balance Details for  {accountIds.Count} Account Ids");

            for (var i = 0; i < accountIds.Count - 1; i++)
            {
                var levyAccountsToUpdate = accountIds.Skip(i).Take(batchSize).ToList();

                logger.LogVerbose($"Processing {batchSize} Batch of Levy Accounts Details. Account Ids {string.Join(",",levyAccountsToUpdate)}");

                var levyAccountModels = await GetLevyAccountsBalance(levyAccountsToUpdate);
                
                await BatchUpdateLevyAccounts(cancellationToken, levyAccountModels);
                

                i += batchSize;
            }
            
        }

        private async Task BatchUpdateLevyAccounts(CancellationToken cancellationToken, List<LevyAccountModel> levyAccountModels)
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await repository
                        .DeletedLevyAccountByIdsAsync(levyAccountModels.Select(x => x.AccountId).ToList(), cancellationToken)
                        .ConfigureAwait(false);

                    await Task.WhenAll(levyAccountModels.Select(x => bulkWriter.Write(x, cancellationToken)))
                        .ConfigureAwait(false);

                    await bulkWriter.Flush(cancellationToken).ConfigureAwait(false);

                    logger.LogVerbose($"Successfully Added  {levyAccountModels.Count} Batch of Levy Accounts Details");

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Error while Adding  {levyAccountModels.Count} Batch of Levy Accounts Details", e);
            }
        }

        private async Task<List<LevyAccountModel>> GetLevyAccountsBalance(List<long> levyPayingAccountIds, List<long> levyAccountsToUpdate)
        {
            var levyAccountModels = new List<LevyAccountModel>();

            foreach (var accountId in levyAccountsToUpdate)
            {
                try
                {
                    logger.LogInfo($"Now trying to retrieve Account Balance Details for AccountId {accountId}");

                    var accountDetail = await accountApiClient.GetAccount(accountId).ConfigureAwait(false);
                    var newLevyAccountModel = new LevyAccountModel
                    {
                        AccountId = accountId,
                        IsLevyPayer = levyPayingAccountIds.Contains(accountId) , // TODO confirm 
                        AccountName = accountDetail.DasAccountName,
                        Balance = accountDetail.Balance,
                        TransferAllowance = accountDetail.RemainingTransferAllowance
                    };
                    levyAccountModels.Add(newLevyAccountModel);

                    logger.LogInfo($"Successfully retrieved Account Balance Details for AccountId {accountId}");
                }
                catch (Exception e)
                {
                    logger.LogError($"Error while retrieving Account Balance Details for AccountId {accountId}", e);
                }

                logger.LogInfo($"Successfully Refreshed All Accounts Balance Details");
            }

            return levyAccountModels;
        }
    }
}
