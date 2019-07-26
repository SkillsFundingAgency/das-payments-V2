using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Repositories
{
    public interface ILevyFundingSourceIntegrationRepository
    {
        Task<List<long>> GetAccountIds(CancellationToken cancellationToken = default(CancellationToken));
        Task<LevyAccountModel> GetLevyAccount(long accountId, CancellationToken cancellationToken = default(CancellationToken));
        Task AddLevyAccounts(List<LevyAccountModel> levyAccounts, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateLevyAccounts(List<LevyAccountModel> levyAccounts, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class LevyFundingSourceIntegrationRepository : ILevyFundingSourceIntegrationRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public LevyFundingSourceIntegrationRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<List<long>> GetAccountIds(CancellationToken cancellationToken = default(CancellationToken))
        {
            var accountIds = await dataContext
                .Apprenticeship
                .Select(x => x.AccountId)
                .Distinct()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var transferAccountIds = await dataContext
                .Apprenticeship
                .Where(o => o.TransferSendingEmployerAccountId.HasValue && o.TransferSendingEmployerAccountId.Value != 0)
                .Select(o => o.TransferSendingEmployerAccountId.Value)
                .Distinct()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            accountIds.AddRange(transferAccountIds);

            return accountIds;
        }

        public async Task<LevyAccountModel> GetLevyAccount(long accountId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var levyAccount = await dataContext.LevyAccount
                 .FirstOrDefaultAsync(x => x.AccountId == accountId, cancellationToken)
                 .ConfigureAwait(false);

            return levyAccount;
        }


        public async Task AddLevyAccounts(List<LevyAccountModel> levyAccounts, CancellationToken cancellationToken = default(CancellationToken))
        {
           await dataContext.LevyAccount
                .AddRangeAsync(levyAccounts, cancellationToken)
                .ConfigureAwait(false);

           await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateLevyAccounts(List<LevyAccountModel> levyAccounts, CancellationToken cancellationToken = default(CancellationToken))
        {
             dataContext.LevyAccount.UpdateRange(levyAccounts);
            await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

    }
}