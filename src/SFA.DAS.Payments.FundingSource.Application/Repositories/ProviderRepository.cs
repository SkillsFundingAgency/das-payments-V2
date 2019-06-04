using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;

namespace SFA.DAS.Payments.FundingSource.Application.Repositories
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public ProviderRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task<List<long>> GetApprenticeshipEmployers(long ukprn)
        {
            var accounts = await dataContext.Apprenticeship
                .Where(apprenticeship => apprenticeship.Ukprn == ukprn)
                .Select(apprenticeship => apprenticeship.AccountId)
                .Distinct()
                .ToListAsync()
                .ConfigureAwait(false);

            var senderAccounts = await dataContext.Apprenticeship
                .Where(apprenticeship => apprenticeship.Ukprn == ukprn 
                                         && apprenticeship.TransferSendingEmployerAccountId != null 
                                         && apprenticeship.TransferSendingEmployerAccountId != apprenticeship.AccountId)
                .Select(apprenticeship => apprenticeship.TransferSendingEmployerAccountId)
                .Distinct()
                .ToListAsync()
                .ConfigureAwait(false);

            accounts.AddRange(senderAccounts.Select(id => id.Value));
            return accounts.Distinct().ToList();
        }
    }
}