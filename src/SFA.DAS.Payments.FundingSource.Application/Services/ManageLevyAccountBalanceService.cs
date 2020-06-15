using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public interface IManageLevyAccountBalanceService
    {
        Task RefreshLevyAccountDetails(int pageNumber, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> GetTotalPageSize();
    }

    public class ManageLevyAccountBalanceService : IManageLevyAccountBalanceService
    {
        private readonly IAccountApiClient accountApiClient;
        private readonly IPaymentLogger logger;
        private readonly ILevyAccountBulkCopyRepository levyAccountBulkWriter;
        private readonly ILevyFundingSourceRepository levyFundingSourceRepository;
        private readonly int batchSize;
        private readonly IEndpointInstanceFactory endpointInstanceFactory;

        public ManageLevyAccountBalanceService(IAccountApiClient accountApiClient,
            IPaymentLogger logger,
            ILevyAccountBulkCopyRepository levyAccountBulkWriter,
            ILevyFundingSourceRepository levyFundingSourceRepository,
            int batchSize,
            IEndpointInstanceFactory endpointInstanceFactory)
        {
            this.accountApiClient = accountApiClient ?? throw new ArgumentNullException(nameof(accountApiClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.levyAccountBulkWriter = levyAccountBulkWriter ?? throw new ArgumentNullException(nameof(levyAccountBulkWriter));
            this.levyFundingSourceRepository = levyFundingSourceRepository ?? throw new ArgumentNullException(nameof(levyFundingSourceRepository));
            this.batchSize = batchSize;
            this.endpointInstanceFactory = endpointInstanceFactory ?? throw new ArgumentNullException(nameof(endpointInstanceFactory));
        }

        public async Task RefreshLevyAccountDetails(int pageNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.LogInfo("Now Trying to Refresh All Accounts Balance Details");

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var pagedAccountsRecords = await accountApiClient.GetPageOfAccounts(pageNumber, batchSize).ConfigureAwait(false);
                var pagedLevyAccountModels = MapToLevyAccountModel(pagedAccountsRecords);
                await BatchUpdateLevyAccounts(pagedLevyAccountModels, cancellationToken).ConfigureAwait(false);
                await PublishEmployerEvents(pagedLevyAccountModels, cancellationToken).ConfigureAwait(false);

                logger.LogInfo($"Successfully retrieved Account Balance Details for Page {pageNumber} of Levy Accounts");
            }
            catch (Exception e)
            {
                logger.LogError($"Error while retrieving Account Balance Details for Page {pageNumber} of Levy Accounts", e);
            }
        }

        public async Task<int> GetTotalPageSize()
        {
            try
            {
                var pagedAccountsRecord = await accountApiClient.GetPageOfAccounts(1, batchSize).ConfigureAwait(false);
                var totalPages = pagedAccountsRecord.TotalPages;

                logger.LogInfo($"Total Levy Account to process {totalPages} ");

                return totalPages;
            }
            catch (Exception e)
            {
                logger.LogError("Error while trying to get Total number of Levy Accounts", e);
                throw;
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


        private async Task PublishEmployerEvents(List<LevyAccountModel> accountModels,CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.LogDebug(
                $"{nameof(PublishEmployerEvents)} for accounts: {String.Join(",",accountModels.Select(x=>x.AccountId))}");

            var storedEmployers =
               await levyFundingSourceRepository.GetCurrentEmployerStatus(accountModels.Select(x => x.AccountId).ToList(),cancellationToken);

            var accountsWithChangedLevyFlags = accountModels
                .Where(x => !storedEmployers.Contains((x.AccountId, x.IsLevyPayer))).ToList();

            var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
            List<Task> publishEvents = new List<Task>();

            publishEvents.AddRange(accountsWithChangedLevyFlags.Where(x=> !x.IsLevyPayer).Select(employer => endpointInstance.Publish(new FoundNotLevyPayerEmployerAccount { AccountId = employer.AccountId })));
            publishEvents.AddRange(accountsWithChangedLevyFlags.Where(x=> x.IsLevyPayer).Select(employer => endpointInstance.Publish(new FoundLevyPayerEmployerAccount { AccountId = employer.AccountId })));

            await Task.WhenAll(publishEvents).ConfigureAwait(false);

            var accountIds = string.Join(",", accountsWithChangedLevyFlags.Select(x=>x.AccountId));
            var totalsString =
                $"{accountsWithChangedLevyFlags.Count(x => x.IsLevyPayer)} Is Levy and {accountsWithChangedLevyFlags.Count(x => !x.IsLevyPayer)} Non Levy accounts";

            logger.LogInfo($"Successfully Published EmployerAccount event for  Account Ids: {accountIds}. Published {totalsString}");
        }
    }
}
