using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Cache
{
    public class IlrSubmissionCache : IDataCache<ReceivedProviderEarningsEvent>
    {
        private readonly IReliableStateManagerTransactionProvider transactionProvider;
        private readonly IReliableDictionary<string, ReceivedProviderEarningsEvent> state;
        
        public IlrSubmissionCache(IReliableStateManagerProvider stateManagerProvider, IReliableStateManagerTransactionProvider transactionProvider)
        {
            if (stateManagerProvider == null) throw new ArgumentNullException(nameof(stateManagerProvider));
            this.transactionProvider = transactionProvider ?? throw new ArgumentNullException(nameof(transactionProvider));

            state = stateManagerProvider.Current.GetOrAddAsync<IReliableDictionary<string, ReceivedProviderEarningsEvent>>(transactionProvider.Current, "SubmissionCache").Result;
        }

        public async Task<bool> Contains(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await state.ContainsKeyAsync(transactionProvider.Current, key).ConfigureAwait(false);
        }

        public async Task Add(string key, ReceivedProviderEarningsEvent entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            await state.AddAsync(transactionProvider.Current, key, entity, TimeSpan.FromSeconds(4), cancellationToken).ConfigureAwait(false);
        }

        public async Task AddOrReplace(string key, ReceivedProviderEarningsEvent entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            await state.AddOrUpdateAsync(transactionProvider.Current, key, entity, (newKey, ilr) => ilr, TimeSpan.FromSeconds(4), cancellationToken).ConfigureAwait(false);
        }

        public async Task<ConditionalValue<ReceivedProviderEarningsEvent>> TryGet(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var value = await state.TryGetValueAsync(transactionProvider.Current, key, TimeSpan.FromSeconds(2), cancellationToken).ConfigureAwait(false);
            return new ConditionalValue<ReceivedProviderEarningsEvent>(value.HasValue, value.Value);
        }

        public async Task Clear(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (await Contains(key, cancellationToken).ConfigureAwait(false))
                await state.TryRemoveAsync(transactionProvider.Current, key, TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
        }
    }
}