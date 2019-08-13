using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ServiceFabric.Core;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Cache
{
    public class PaymentExportProgressCache : IPaymentExportProgressCache
    {
        private readonly IReliableStateManagerTransactionProvider transactionProvider;
        private readonly IReliableStateManagerProvider stateManagerProvider;
        private IReliableDictionary2<string, int> reliableDictionary;
        private static readonly object LockObject = new object();

        private const string Key = "payments-export-progress";

        public PaymentExportProgressCache(IReliableStateManagerProvider stateManagerProvider, IReliableStateManagerTransactionProvider transactionProvider)
        {
            this.transactionProvider = transactionProvider ?? throw new ArgumentNullException(nameof(transactionProvider));
            this.stateManagerProvider = stateManagerProvider ?? throw new ArgumentNullException(nameof(stateManagerProvider));
        }

        public async Task<int> GetPage(short academicYear, byte collectionPeriod)
        {
            var key = CreateKey(academicYear, collectionPeriod);
            var state = await GetState();

            // Don't want to tie up a transaction for what could be a long running process
            using (var transaction = stateManagerProvider.Current.CreateTransaction())
            {
                var result = await state.GetOrAddAsync(transactionProvider.Current, key, 0).ConfigureAwait(false);
                await transaction.CommitAsync();
                return result;
            }
        }

        public async Task<int> IncrementPage(short academicYear, byte collectionPeriod)
        {
            var key = CreateKey(academicYear, collectionPeriod);
            var state = await GetState();

            for (var i = 0; i < 5; i++)
            {
                // This will be called multiple times between database operations, so this part has
                //  its own transaction to prevent a larger transaction through this process
                using (var transaction = stateManagerProvider.Current.CreateTransaction())
                {
                    var currentValue = await state.TryGetValueAsync(transaction, key, LockMode.Update,
                            TimeSpan.FromSeconds(10), default)
                        .ConfigureAwait(false);
                    if (currentValue.HasValue)
                    {
                        var updateResult = await state.TryUpdateAsync(transaction, key, currentValue.Value + 1, currentValue.Value);
                        if (updateResult)
                        {
                            await transaction.CommitAsync().ConfigureAwait(false);
                            return currentValue.Value + 1;
                        }
                    }
                }
            }

            return 0;
        }

        private async Task<IReliableDictionary2<string, int>> GetState()
        {
            if (reliableDictionary != null)
                return reliableDictionary;

            IReliableDictionary2<string, int> state = null;

            var stateValue = await stateManagerProvider.Current
                .TryGetAsync<IReliableDictionary2<string, int>>(Key)
                .ConfigureAwait(false);

            if (!stateValue.HasValue)
            {
                // this has to be done in a separate transaction. https://github.com/Azure/service-fabric-issues/issues/24
                using (var transaction = stateManagerProvider.Current.CreateTransaction())
                {
                    state = await stateManagerProvider
                        .Current
                        .GetOrAddAsync<IReliableDictionary2<string, int>>(transaction, Key)
                        .ConfigureAwait(false);

                    await transaction.CommitAsync();
                }
            }
            else
            {
                state = stateValue.Value;
            }

            lock (LockObject)
            {
                if (reliableDictionary == null)
                    reliableDictionary = state;
            }

            return reliableDictionary;
        }

        private static string CreateKey(short academicYear, byte collectionPeriod)
        {
            return $"{academicYear}-R{collectionPeriod:D2}";
        }
    }
}
