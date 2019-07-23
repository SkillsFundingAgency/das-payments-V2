using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public interface  IManageLevyAccountBalanceService
    {
        Task UpdateLevyAccountDetails();
    }


    public class ManageLevyAccountBalanceService: IManageLevyAccountBalanceService
    {
        private readonly ILevyFundingSourceRepository repository;

        public ManageLevyAccountBalanceService(ILevyFundingSourceRepository repository)
        {
            this.repository = repository;
        }
        
        public async Task UpdateLevyAccountDetailsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var accountIds = await repository.GetAccountIds(cancellationToken)
                                    .ConfigureAwait(false);

            foreach (var accountId in accountIds)
            {
                
            }


            return Task.CompletedTask;
        }


    }
}
