using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring
{
    public interface IDasLevyAccountBalanceApiWrapper
    {
        Task<List<LevyAccountModel>> GetDasLevyAccountDetails();
    }
    
    public class DasLevyAccountBalanceApiWrapper : IDasLevyAccountBalanceApiWrapper
    {
        private readonly int accountApiBatchSize;
        private readonly IAccountApiClient accountApiClient;
        private readonly IPaymentLogger logger;

        public DasLevyAccountBalanceApiWrapper(int accountApiBatchSize, IAccountApiClient accountApiClient, IPaymentLogger logger)
        {
            this.accountApiBatchSize = accountApiBatchSize;
            this.accountApiClient = accountApiClient;
            this.logger = logger;
        }

        public async Task<List<LevyAccountModel>> GetDasLevyAccountDetails()
        {
            logger.LogInfo("Started Importing DAS Employer Accounts");

            var dasLevyAccountDetails = new List<LevyAccountModel>();

            var totalPages = await GetTotalPageSize();

            for (var pageNumber = 1; pageNumber <= totalPages; pageNumber++)
            {
                var levyAccountDetails = await GetLevyAccountDetails(pageNumber);

                dasLevyAccountDetails.AddRange(levyAccountDetails);
            }

            logger.LogInfo("Finished Importing DAS Employer Accounts");

            return dasLevyAccountDetails;
        }

        private async Task<List<LevyAccountModel>> GetLevyAccountDetails(int pageNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var pagedAccountsRecords = await accountApiClient.GetPageOfAccounts(pageNumber, accountApiBatchSize).ConfigureAwait(false);

                logger.LogInfo($"Successfully retrieved Account Balance Details for Page {pageNumber} of Levy Accounts");

                return MapToLevyAccountModel(pagedAccountsRecords);
            }
            catch (Exception e)
            {
                logger.LogError($"Error while retrieving Account Balance Details for Page {pageNumber} of Levy Accounts", e);
                return new List<LevyAccountModel>();
            }
        }

        private async Task<int> GetTotalPageSize()
        {
            try
            {
                var pagedAccountsRecord = await accountApiClient.GetPageOfAccounts(1, accountApiBatchSize).ConfigureAwait(false);
                var totalPages = pagedAccountsRecord.TotalPages;

                logger.LogInfo($"Total Levy Account to process {totalPages} ");

                return totalPages;
            }
            catch (Exception e)
            {
                logger.LogError("Error while trying to get Total number of Levy Accounts", e);
                return -1;
            }
        }

        private static List<LevyAccountModel> MapToLevyAccountModel(PagedApiResponseViewModel<AccountWithBalanceViewModel> pagedAccountWithBalanceViewModel)
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
