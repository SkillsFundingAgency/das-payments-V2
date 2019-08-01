using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
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
        private readonly ILevyAccountBulkCopyRepository levyAccountBulkWriter;
        private readonly int batchSize;

        public ManageLevyAccountBalanceService(ILevyFundingSourceRepository repository,
            IAccountApiClient accountApiClient,
            IPaymentLogger logger,
            ILevyAccountBulkCopyRepository levyAccountBulkWriter,
            int batchSize)
        {
            this.repository = repository;
            this.accountApiClient = accountApiClient;
            this.logger = logger;
            this.levyAccountBulkWriter = levyAccountBulkWriter;
            this.batchSize = batchSize;
        }

        public async Task RefreshLevyAccountDetails(CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.LogInfo($"Retrieving Non Levy Payers AccountIds");
           
            logger.LogInfo("Now Trying to Refresh All Accounts Balance Details");
            var page = 1;
            int? totalPageSize = null;

            while (totalPageSize == null || page <= totalPageSize)
            {
                try
                {
                    var pagedAccountsRecords = await accountApiClient.GetPageOfAccounts(page, batchSize).ConfigureAwait(false);

                    if (totalPageSize == null)
                    {
                        totalPageSize = pagedAccountsRecords.TotalPages;
                        logger.LogInfo($"Total Levy Account Page Size : {totalPageSize.Value}  for Batch Size :{batchSize}");
                    }
                
                    await UpdateLevyAccountDetails(pagedAccountsRecords, cancellationToken);

                    logger.LogInfo($"Successfully retrieved Account Balance Details for Page {page} of Levy Accounts");
                }
                catch (Exception e)
                {
                    logger.LogError($"Error while retrieving Account Balance Details for Page {page} of Levy Accounts",e);
                }

                page++;
            }
        }

        private async Task UpdateLevyAccountDetails(PagedApiResponseViewModel<AccountWithBalanceViewModel> pagedAccountsRecords, CancellationToken cancellationToken)
        {
            var pagedLevyAccountModels = MapToLevyAccountModel(pagedAccountsRecords);
            await BatchUpdateLevyAccounts(pagedLevyAccountModels, cancellationToken).ConfigureAwait(false);
        }

        private async Task BatchUpdateLevyAccounts(List<LevyAccountModel> levyAccountModels, CancellationToken cancellationToken)
        {
            try
            {
                await Task.WhenAll(levyAccountModels.Select(x => levyAccountBulkWriter.Write(x, cancellationToken)))
                        .ConfigureAwait(false);

                await levyAccountBulkWriter.DeleteAndFlush(levyAccountModels.Select(x => x.AccountId).ToList(), cancellationToken).ConfigureAwait(false);

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
    }
}
