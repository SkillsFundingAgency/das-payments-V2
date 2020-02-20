using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.ServiceFabric.Core;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService
{
    public class InProgressMessageRepository : IInProgressMessageRepository
    {
        private readonly IReliableStateManagerProvider stateManagerProvider;
        private readonly IReliableStateManagerTransactionProvider reliableTransactionProvider;
        private static readonly TimeSpan TransactionTimeout = new TimeSpan(0, 0, 4);

        public const string InProgressMessagesCacheKey = "inprogress_messages";

        public InProgressMessageRepository(IReliableStateManagerProvider stateManagerProvider,
            IReliableStateManagerTransactionProvider reliableTransactionProvider)
        {
            this.stateManagerProvider = stateManagerProvider ?? throw new ArgumentNullException(nameof(stateManagerProvider));
            this.reliableTransactionProvider = reliableTransactionProvider ?? throw new ArgumentNullException(nameof(reliableTransactionProvider));
        }

        private async Task<IReliableDictionary2<Guid, InProgressMessage>> GetInProgressMessagesCollection(long jobId)
        {
            return await stateManagerProvider.Current.GetOrAddAsync<IReliableDictionary2<Guid, InProgressMessage>>(
                $"{InProgressMessagesCacheKey}_{jobId}");
        }

        public async Task<List<InProgressMessage>> GetOrAddInProgressMessages(long jobId, CancellationToken cancellationToken)
        {
            var inProgressCollection = await GetInProgressMessagesCollection(jobId).ConfigureAwait(false);
            var enumerator = (await inProgressCollection.CreateEnumerableAsync(reliableTransactionProvider.Current)).GetAsyncEnumerator();
            var identifiers = new List<InProgressMessage>();

            while (await enumerator.MoveNextAsync(cancellationToken))
            {
                identifiers.Add(enumerator.Current.Value);
            }
            return identifiers;
        }

        public async Task RemoveInProgressMessages(long jobId, List<Guid> messageIdentifiers, CancellationToken cancellationToken)
        {
            var inProgressCollection = await GetInProgressMessagesCollection(jobId).ConfigureAwait(false);
            foreach (var messageIdentifier in messageIdentifiers)
            {
                await inProgressCollection.TryRemoveAsync(reliableTransactionProvider.Current, messageIdentifier,
                        TransactionTimeout, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        public async Task StoreInProgressMessages(long jobId, List<InProgressMessage> inProgressMessages, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var inProgressMessagesCollection = await GetInProgressMessagesCollection(jobId);
            foreach (var inProgressMessage in inProgressMessages)
            {
                await inProgressMessagesCollection.AddOrUpdateAsync(reliableTransactionProvider.Current, inProgressMessage.MessageId,
                        key => inProgressMessage, (key, value) => inProgressMessage, TransactionTimeout,
                        cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}