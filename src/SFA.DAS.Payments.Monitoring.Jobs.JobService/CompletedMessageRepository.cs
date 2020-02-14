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
    public class CompletedMessageRepository : ICompletedMessageRepository
    {
        private readonly IReliableStateManagerProvider stateManagerProvider;
        private readonly IReliableStateManagerTransactionProvider reliableTransactionProvider;
        private static readonly TimeSpan TransactionTimeout = new TimeSpan(0, 0, 4);

        public const string CompletedMessagesCacheKey = "completed_messages";

        public CompletedMessageRepository(IReliableStateManagerProvider stateManagerProvider,
            IReliableStateManagerTransactionProvider reliableTransactionProvider)
        {
            this.stateManagerProvider = stateManagerProvider ?? throw new ArgumentNullException(nameof(stateManagerProvider));
            this.reliableTransactionProvider = reliableTransactionProvider ?? throw new ArgumentNullException(nameof(reliableTransactionProvider));
        }

        private async Task<IReliableDictionary2<Guid, CompletedMessage>> GetCompletedMessagesCollection(long jobId)
        {
            return await stateManagerProvider.Current.GetOrAddAsync<IReliableDictionary2<Guid, CompletedMessage>>(
                $"{CompletedMessagesCacheKey}_{jobId}").ConfigureAwait(false);
        }

        public async Task<List<CompletedMessage>> GetCompletedMessages(long jobId, CancellationToken cancellationToken)
        {
            var completedMessageCollection = await GetCompletedMessagesCollection(jobId).ConfigureAwait(false);
            var enumerator = (await completedMessageCollection.CreateEnumerableAsync(reliableTransactionProvider.Current)).GetAsyncEnumerator();
            var identifiers = new List<CompletedMessage>();

            while (await enumerator.MoveNextAsync(cancellationToken))
            {
                identifiers.Add(enumerator.Current.Value);
            }
            return identifiers;
        }

        public async Task RemoveCompletedMessages(long jobId, List<Guid> completedMessages, CancellationToken cancellationToken)
        {
            var completedMessagesCollection = await GetCompletedMessagesCollection(jobId).ConfigureAwait(false);
            foreach (var completedMessage in completedMessages)
            {
                await completedMessagesCollection.TryRemoveAsync(reliableTransactionProvider.Current, completedMessage,
                    TransactionTimeout, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task StoreCompletedMessage(CompletedMessage completedMessage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var completedMessagesCollection =
                await GetCompletedMessagesCollection(completedMessage.JobId).ConfigureAwait(false);
            await completedMessagesCollection.AddOrUpdateAsync(reliableTransactionProvider.Current,
                    completedMessage.MessageId,
                    completedMessage, (key, value) => completedMessage, TransactionTimeout, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}