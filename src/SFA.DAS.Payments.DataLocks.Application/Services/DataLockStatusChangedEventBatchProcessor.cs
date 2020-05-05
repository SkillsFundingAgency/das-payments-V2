using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class DataLockStatusChangedEventBatchProcessor : IBatchProcessor<PriceEpisodeStatusChange>
    {
        private readonly IBatchedDataCache<PriceEpisodeStatusChange> cache;
        private readonly IPaymentLogger logger;
        private readonly IBulkWriter<LegacyDataLockEvent> dataLockEventWriter;
        private readonly IBulkWriter<LegacyDataLockEventCommitmentVersion> dataLockEventCommitmentVersionWriter;
        private readonly IBulkWriter<LegacyDataLockEventError> dataLockEventErrorWriter;
        private readonly IBulkWriter<LegacyDataLockEventPeriod> dataLockEventPeriodWriter;

        public DataLockStatusChangedEventBatchProcessor(
            IBatchedDataCache<PriceEpisodeStatusChange> cache,
            IPaymentLogger logger,
            IBulkWriter<LegacyDataLockEvent> dataLockEventWriter,
            IBulkWriter<LegacyDataLockEventCommitmentVersion> dataLockEventCommitmentVersionWriter,
            IBulkWriter<LegacyDataLockEventError> dataLockEventErrorWriter,
            IBulkWriter<LegacyDataLockEventPeriod> dataLockEventPeriodWriter)
        {
            this.cache = cache;
            this.logger = logger;
            this.dataLockEventWriter = dataLockEventWriter;
            this.dataLockEventCommitmentVersionWriter = dataLockEventCommitmentVersionWriter;
            this.dataLockEventErrorWriter = dataLockEventErrorWriter;
            this.dataLockEventPeriodWriter = dataLockEventPeriodWriter;
        }

        public async Task<int> Process(int batchSize, CancellationToken cancellationToken)
        {
            logger.LogVerbose("Processing batch.");
            var batch = await cache.GetPayments(batchSize, cancellationToken).ConfigureAwait(false);
            if (batch.Count < 1)
            {
                logger.LogVerbose("No records found to process.");
                return 0;
            }

            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var changeEvent in batch)
                    {
                        await SaveDataLockEvent(cancellationToken, changeEvent);

                        foreach(var period in changeEvent.Periods)
                            await SaveEventPeriods(period, changeEvent, cancellationToken);

                        foreach(var commitment in changeEvent.CommitmentVersions)
                            await SaveCommitmentVersion(commitment, changeEvent, cancellationToken);

                        foreach(var error in changeEvent.Errors)
                            await SaveErrorCode(error, changeEvent, cancellationToken);

                        logger.LogDebug(
                            $"Saved PriceEpisodeStatusChange event {changeEvent.DataLock.DataLockEventId} for UKPRN {changeEvent.DataLock.UKPRN}. " +
                            $"Commitment versions: {changeEvent.CommitmentVersions.Length}, " +
                            $"periods: {changeEvent.Periods.Length}, errors: {changeEvent.Errors.Length}");
                    }

                    await dataLockEventWriter.Flush(cancellationToken);
                    await dataLockEventCommitmentVersionWriter.Flush(cancellationToken);
                    await dataLockEventPeriodWriter.Flush(cancellationToken);
                    await dataLockEventErrorWriter.Flush(cancellationToken);

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Error saving batch of DataLockStatusChanged events. Error: {e.Message}", e);
                throw;
            }

            return batch.Count;
        }

        private async Task SaveErrorCode(LegacyDataLockEventError error, PriceEpisodeStatusChange dataLockStatusChangedEvent, CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Saving DataLockEventError {error} for legacy DataLockEvent {dataLockStatusChangedEvent.DataLock.DataLockEventId} for UKPRN {dataLockStatusChangedEvent.DataLock.UKPRN}");
            await dataLockEventErrorWriter.Write(error, cancellationToken);
        }

        private async Task SaveEventPeriods(LegacyDataLockEventPeriod period, PriceEpisodeStatusChange dataLockStatusChangedEvent, CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Saving DataLockEventPeriod {period.CollectionPeriodName} for legacy DataLockEvent {dataLockStatusChangedEvent.DataLock.DataLockEventId} for UKPRN {dataLockStatusChangedEvent.DataLock.UKPRN}");
            await dataLockEventPeriodWriter.Write(period, cancellationToken).ConfigureAwait(false);
        }

        private async Task SaveCommitmentVersion(LegacyDataLockEventCommitmentVersion commitment, PriceEpisodeStatusChange dataLockStatusChangedEvent, CancellationToken cancellationToken)
        {
            logger.LogVerbose($"Saving DataLockEventCommitmentVersion {commitment} for PriceEpisodeStatusChange {dataLockStatusChangedEvent.DataLock.DataLockEventId} for UKPRN {dataLockStatusChangedEvent.DataLock.UKPRN}");
            await dataLockEventCommitmentVersionWriter.Write(commitment, cancellationToken).ConfigureAwait(false);
        }

        private async Task SaveDataLockEvent(CancellationToken cancellationToken, PriceEpisodeStatusChange priceEpisodeStatus)
        {
            logger.LogVerbose($"Saving legacy DataLockEvent {priceEpisodeStatus.DataLock.DataLockEventId} for UKPRN {priceEpisodeStatus.DataLock.UKPRN}");
            await dataLockEventWriter.Write(priceEpisodeStatus.DataLock, cancellationToken);
        }

        public static string TrimUkprnFromIlrFileNameLimitToValidLength(string input)
        {
            const int validLength = 50;

            if (input == null)
            {
                return string.Empty;
            }

            if (input.Length > 9)
            {
                input = input.Substring(9);

                if (input.Length > validLength)
                {
                    return input.Substring(0, validLength);
                }
            }

            return input;
        }
    }
}
