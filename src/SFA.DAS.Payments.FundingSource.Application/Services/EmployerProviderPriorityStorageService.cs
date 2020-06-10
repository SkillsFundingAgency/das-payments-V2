using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class EmployerProviderPriorityStorageService : IEmployerProviderPriorityStorageService
    {
        private readonly IFundingSourceDataContext dataContext;

        public EmployerProviderPriorityStorageService(IFundingSourceDataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task StoreEmployerProviderPriority(EmployerChangedProviderPriority providerPriorityEvent)
        {
            var order = 1;
            var paymentPriorities = new List<EmployerProviderPriorityModel>();
            foreach (var providerUkprn in providerPriorityEvent.OrderedProviders)
            {
                paymentPriorities.Add(new EmployerProviderPriorityModel
                {
                    Ukprn = providerUkprn,
                    EmployerAccountId = providerPriorityEvent.EmployerAccountId,
                    Order = order
                });

                order++;
            }

            await dataContext.ReplaceEmployerProviderPriorities(providerPriorityEvent.EmployerAccountId, paymentPriorities,
                CancellationToken.None);
            await dataContext.SaveChanges(CancellationToken.None);
        }
    }
}