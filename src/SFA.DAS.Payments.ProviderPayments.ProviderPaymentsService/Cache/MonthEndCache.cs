using Microsoft.ServiceFabric.Data.Collections;
using SFA.DAS.Payments.ProviderPayments.Model;
using SFA.DAS.Payments.ServiceFabric.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Cache
{
    public class MonthEndCache : IMonthEndCache
    {
        private readonly IReliableStateManagerTransactionProvider transactionProvider;
        private readonly IReliableStateManagerProvider stateManagerProvider;
        private IReliableDictionary2<string, MonthEndDetails> reliableDictionary;
        private readonly object lockObject = new object();

        public MonthEndCache(IReliableStateManagerProvider stateManagerProvider, IReliableStateManagerTransactionProvider transactionProvider)
        {
            this.transactionProvider = transactionProvider ?? throw new ArgumentNullException(nameof(transactionProvider));
            this.stateManagerProvider = stateManagerProvider ?? throw new ArgumentNullException(nameof(stateManagerProvider));
        }

        public async Task<bool> Exists(long ukprn, short academicYear, byte collectionPeriod, CancellationToken cancellationToken = default(CancellationToken))
        {
            var key = CreateKey(ukprn, academicYear, collectionPeriod);
            var state = await GetState();

            return await state
                .ContainsKeyAsync(transactionProvider.Current, key, TimeSpan.FromSeconds(4), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<MonthEndDetails> GetMonthEndDetails(long ukprn, short academicYear, byte collectionPeriod, CancellationToken cancellationToken = default(CancellationToken))
        {
            var key = CreateKey(ukprn, academicYear, collectionPeriod);
            var state = await GetState();
            var value = await state.TryGetValueAsync(transactionProvider.Current, key, TimeSpan.FromSeconds(4), cancellationToken).ConfigureAwait(false);
            return value.Value;
        }

        private async Task<IReliableDictionary2<string, MonthEndDetails>> GetState()
        {
            if (reliableDictionary != null) 
                return reliableDictionary;

            IReliableDictionary2<string, MonthEndDetails> state = null;

            var stateValue = await stateManagerProvider.Current
                .TryGetAsync<IReliableDictionary2<string, MonthEndDetails>>("MonthEndCache")
                .ConfigureAwait(false);

            if (!stateValue.HasValue)
            {
                // this has to be done in a separate transaction. https://github.com/Azure/service-fabric-issues/issues/24
                using (var transaction = stateManagerProvider.Current.CreateTransaction())
                {
                    state = await stateManagerProvider
                        .Current
                        .GetOrAddAsync<IReliableDictionary2<string, MonthEndDetails>>(transaction, "MonthEndCache")
                        .ConfigureAwait(false);

                    await transaction.CommitAsync();
                }
            }
            else
            {
                state = stateValue.Value;
            }

            lock (lockObject)
            {
                if (reliableDictionary == null)
                    reliableDictionary = state;
            }

            return reliableDictionary;
        }

        private static string CreateKey(long ukprn, short academicYear, byte collectionPeriod)
        {
            return $"{ukprn}-{academicYear}-{collectionPeriod}";
        }
    }
}