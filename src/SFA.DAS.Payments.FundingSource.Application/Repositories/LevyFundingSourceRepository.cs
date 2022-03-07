using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Repositories
{
    public class LevyFundingSourceRepository : ILevyFundingSourceRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public LevyFundingSourceRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<LevyAccountModel> GetLevyAccount(long employerAccountId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var levyAccounts = await dataContext.LevyAccount.AsNoTracking()
                .Where(levyAccount => levyAccount.AccountId == employerAccountId)
                .ToListAsync(cancellationToken);

            if (levyAccounts.Count > 1)
                throw new InvalidOperationException($"Found multiple ({levyAccounts.Count}) levy accounts with the same account id: {employerAccountId}");

            if (levyAccounts.Count == 0)
                throw new InvalidOperationException($"Found no employer accounts for account id: {employerAccountId}");

            return levyAccounts.First();
        }

        public async Task<List<long>> GetEmployerAccounts(CancellationToken cancellationToken)
        {
            var transferSenders = (await dataContext.Apprenticeship
                .Where(apprenticeship => apprenticeship.TransferSendingEmployerAccountId != null && apprenticeship.TransferSendingEmployerAccountId != 0)
                .Select(apprenticeship => apprenticeship.TransferSendingEmployerAccountId)
                .Distinct()
                .ToListAsync(cancellationToken))
                .Select(accountId => accountId.Value)
                .ToList();
            var accounts = await dataContext.Apprenticeship
                .Select(apprenticeship => apprenticeship.AccountId)
                .Distinct()
                .ToListAsync(cancellationToken);

            accounts.AddRange(transferSenders);
            return accounts.Distinct().ToList();
        }

        public async Task<List<(long, bool)>> GetCurrentEmployerStatus(List<long> employerIds,CancellationToken cancellationToken = default(CancellationToken))
        {
            var accountStatuses = await  dataContext.LevyAccount.Where(x => employerIds.Contains(x.AccountId))
                .Select(x => new {x.AccountId, x.IsLevyPayer}).ToListAsync(cancellationToken);
            return accountStatuses.Select(x => (x.AccountId, x.IsLevyPayer)).ToList();
        }

        public async Task SaveLevyAccountAuditModel(long accountId, short academicYear, byte collectionPeriod, decimal sourceLevyAccountBalance, decimal adjustedLevyAccountBalance, decimal sourceTransferAllowance, decimal adjustedTransferAllowance, bool isLevyPayer)
        {
            var model = new LevyAccountAuditModel
            {
                AccountId = accountId,
                AcademicYear = academicYear,
                CollectionPeriod = collectionPeriod,
                SourceLevyAccountBalance = sourceLevyAccountBalance,
                AdjustedLevyAccountBalance = adjustedLevyAccountBalance,
                SourceTransferAllowance = sourceTransferAllowance,
                AdjustedTransferAllowance = adjustedTransferAllowance,
                IsLevyPayer = isLevyPayer
            };

            dataContext.LevyAccountAudits.Add(model);

            await dataContext.SaveChangesAsync();
        }

        public async Task<List<Tuple<long,long?>>> GetEmployerAccountsByUkprn(long ukprn, CancellationToken cancellationToken = default(CancellationToken))
        {
            var accounts = await dataContext.Apprenticeship.AsNoTracking()
                .Where(apprenticeship => apprenticeship.Ukprn == ukprn)
                .Select(apprenticeship => new
                {
                    apprenticeship.AccountId,
                    apprenticeship.TransferSendingEmployerAccountId
                })
                .Distinct()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return accounts.Select(x => new Tuple<long, long?>(x.AccountId, x.TransferSendingEmployerAccountId)).ToList();
        }
    }
}