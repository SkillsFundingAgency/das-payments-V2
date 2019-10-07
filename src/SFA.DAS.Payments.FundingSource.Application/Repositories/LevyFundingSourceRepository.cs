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

        public async Task<List<EmployerProviderPriorityModel>> GetPaymentPriorities(long employerAccountId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var paymentPriorities = await dataContext.EmployerProviderPriority.AsNoTracking()
                .Where(paymentPriority => paymentPriority.EmployerAccountId == employerAccountId)
                .ToListAsync(cancellationToken);

            return paymentPriorities;
        }

        public async Task ReplaceEmployerProviderPriorities(long employerAccountId, List<EmployerProviderPriorityModel> paymentPriorityModels, CancellationToken cancellationToken = default(CancellationToken))
        {
            var previousEmployerPriorities = await GetPaymentPriorities(employerAccountId, cancellationToken).ConfigureAwait(false);
            dataContext.EmployerProviderPriority.RemoveRange(previousEmployerPriorities);

            await dataContext.EmployerProviderPriority
                .AddRangeAsync(paymentPriorityModels, cancellationToken)
                .ConfigureAwait(false);

            await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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

        public async Task<List<Tuple<long,long?>>> GetEmployerAccountsByUkprn(long ukprn, CancellationToken cancellationToken = default(CancellationToken))
        {
            var accounts = await dataContext.Apprenticeship.AsNoTracking()
                .Where(apprenticeship => apprenticeship.Ukprn == ukprn)
                .Select (apprenticeship => new {
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