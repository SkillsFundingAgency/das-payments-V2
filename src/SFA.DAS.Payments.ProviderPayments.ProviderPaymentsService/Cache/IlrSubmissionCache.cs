using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Audit.Application.ServiceFabric.Infrastructure;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Cache
{
    public class IlrSubmissionCache : IDataCache<IlrSubmittedEvent>
    {
        private readonly IReliableStateManagerTransactionProvider transactionProvider;
        private readonly IReliableDictionary<string, IlrSubmittedEvent> state;
        
        public IlrSubmissionCache(IReliableStateManagerProvider stateManagerProvider, IReliableStateManagerTransactionProvider transactionProvider)
        {
            if (stateManagerProvider == null) throw new ArgumentNullException(nameof(stateManagerProvider));
            this.transactionProvider = transactionProvider ?? throw new ArgumentNullException(nameof(transactionProvider));

            state = stateManagerProvider.Current.GetOrAddAsync<IReliableDictionary<string, IlrSubmittedEvent>>("SubmissionCache").Result;
        }

        public async Task<bool> Contains(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await state.ContainsKeyAsync(transactionProvider.Current, key);
        }

        public async Task Add(string key, IlrSubmittedEvent entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            await state.AddAsync(transactionProvider.Current, key, entity, TimeSpan.FromSeconds(4), cancellationToken);
        }

        public async Task AddOrReplace(string key, IlrSubmittedEvent entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            await state.AddOrUpdateAsync(transactionProvider.Current, key, entity, (newKey, ilr) => ilr, TimeSpan.FromSeconds(4), cancellationToken).ConfigureAwait(false);
        }

        public async Task<Payments.Application.Repositories.ConditionalValue<IlrSubmittedEvent>> TryGet(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var value = await state.TryGetValueAsync(transactionProvider.Current, key, TimeSpan.FromSeconds(4), cancellationToken);
            return new Payments.Application.Repositories.ConditionalValue<IlrSubmittedEvent>(value.HasValue, value.Value);
        }

        public async Task Clear(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (await Contains(key, cancellationToken))
                await state.TryRemoveAsync(transactionProvider.Current, key, TimeSpan.FromSeconds(5), cancellationToken);
        }
    }
}