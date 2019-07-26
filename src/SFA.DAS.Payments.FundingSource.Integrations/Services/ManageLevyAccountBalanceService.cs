using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Integrations.Services
{
    public interface IManageLevyAccountBalanceService
    {
        Task RefreshLevyAccountDetails(CancellationToken cancellationToken = default(CancellationToken));
    }

    public class ManageLevyAccountBalanceService : IManageLevyAccountBalanceService
    {
        private readonly ILevyFundingSourceIntegrationRepository repository;
        private readonly IAccountApiClient accountApiClient;
        private readonly IPaymentLogger logger;
        private readonly int batchSize;

        public ManageLevyAccountBalanceService(ILevyFundingSourceIntegrationRepository repository, IAccountApiClient accountApiClient, IPaymentLogger logger, int batchSize)
        {
            this.repository = repository;
            this.accountApiClient = accountApiClient;
            this.logger = logger;
            this.batchSize = batchSize;

        }

        public async Task RefreshLevyAccountDetails(CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.LogInfo($"Now Trying to Refresh All Accounts Balance Details");

            var accountIds = await repository.GetAccountIds(cancellationToken).ConfigureAwait(false);

            logger.LogInfo($"Retrieving Account Balance Details for  {accountIds.Count} Account Ids");

            var newLevyAccountModels = new List<LevyAccountModel>();
            var updatedLevyAccountModels = new List<LevyAccountModel>();

            foreach (var accountId in accountIds)
            {
                await AddOrUpdateLevyAccounts(batchSize, newLevyAccountModels, updatedLevyAccountModels, cancellationToken);

                try
                {
                    logger.LogInfo($"Now trying to retrieve Account Balance Details for AccountId {accountId}");

                    var accountDetail = await accountApiClient.GetAccount(accountId).ConfigureAwait(false);

                    var currentLevyAccount = await repository.GetLevyAccount(accountId, cancellationToken).ConfigureAwait(false);

                    if (currentLevyAccount == null)
                    {
                        var newLevyAccountModel = new LevyAccountModel
                        {
                            AccountId = accountId,
                            IsLevyPayer = true,  // TODO confirm 
                            AccountName = accountDetail.DasAccountName,
                            Balance = accountDetail.Balance,
                            TransferAllowance = accountDetail.RemainingTransferAllowance
                        };

                        newLevyAccountModels.Add(newLevyAccountModel);
                    }
                    else
                    {
                        currentLevyAccount.AccountName = accountDetail.DasAccountName;
                        currentLevyAccount.Balance = accountDetail.Balance;
                        currentLevyAccount.TransferAllowance = accountDetail.RemainingTransferAllowance;

                        updatedLevyAccountModels.Add(currentLevyAccount);
                    }

                    logger.LogInfo($"Successfully retrieved Account Balance Details for AccountId {accountId}");
                }
                catch (Exception e)
                {
                    logger.LogError($"Error while retrieving Account Balance Details for AccountId {accountId}", e);
                }

                logger.LogInfo($"Successfully Refreshed All Accounts Balance Details");
            }

            await AddOrUpdateLevyAccounts(0, newLevyAccountModels, updatedLevyAccountModels, cancellationToken);

        }

        private async Task AddOrUpdateLevyAccounts(int processBatchSize, List<LevyAccountModel> newLevyAccountModels, List<LevyAccountModel> updatedLevyAccountModels, CancellationToken cancellationToken)
        {
            try
            {
                if (newLevyAccountModels.Count > processBatchSize)
                {
                    logger.LogVerbose($"Adding New {newLevyAccountModels.Count} Batch of Levy Accounts Details");

                    await repository.AddLevyAccounts(newLevyAccountModels, cancellationToken);
                    newLevyAccountModels.Clear();

                    logger.LogVerbose($"Successfully Added New  {newLevyAccountModels.Count} Batch of Levy Accounts Details");
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Error while Adding New {newLevyAccountModels.Count} Batch of Levy Accounts Details", e);
            }

            try
            {
                if (updatedLevyAccountModels.Count > processBatchSize)
                {
                    logger.LogVerbose($"Updating {newLevyAccountModels.Count} Batch of Levy Accounts Details");
                    await repository.UpdateLevyAccounts(updatedLevyAccountModels, cancellationToken);
                    updatedLevyAccountModels.Clear();
                    logger.LogVerbose($"Successfully Updated {newLevyAccountModels.Count} Batch of Levy Accounts Details");
                }

            }
            catch (Exception e)
            {
                logger.LogError($"Error while Updating {newLevyAccountModels.Count} Batch of Levy Accounts Details", e);
            }

         
        }
    }
}
