using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class DataLockStatusChangedEventBatchProcessor : IBatchProcessor<DataLockStatusChanged>
    {
        private readonly IBatchedDataCache<DataLockStatusChanged> cache;
        private readonly IPaymentLogger logger;
        private readonly IBulkWriter<LegacyDataLockEvent> dataLockEventWriter;
        private readonly IBulkWriter<LegacyDataLockEventCommitmentVersion> dataLockEventCommitmentVersionWriter;
        private readonly IBulkWriter<LegacyDataLockEventError> dataLockEventErrorWriter;
        private readonly IBulkWriter<LegacyDataLockEventPeriod> dataLockEventPeriodWriter;

        public DataLockStatusChangedEventBatchProcessor(
            IBatchedDataCache<DataLockStatusChanged> cache,
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

            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var dataLockStatusChangedEvent in batch)
                    {
                        int savedEvents = 0, savedCommitmentVersions = 0, savedPeriods = 0, savedErrors = 0;
                        var isError = dataLockStatusChangedEvent is DataLockStatusChangedToFailed || dataLockStatusChangedEvent is DataLockFailureChanged;

                        // TODO: group by commitment ID

                        // v1 doesn't use delivery period, use price episode instead
                        var priceEpisodes = dataLockStatusChangedEvent.TransactionTypesAndPeriods.SelectMany(p => p.Value).GroupBy(p => p.PriceEpisodeIdentifier).Select(g => g.First()).ToList();

                        foreach (var priceEpisode in priceEpisodes)
                        {
                            await SaveDataLockEvent(cancellationToken, dataLockStatusChangedEvent, priceEpisode).ConfigureAwait(false);
                            savedEvents++;

                            var commitmentVersionIds = GetCommitmentVersions(isError, priceEpisode);

                            foreach (var commitmentVersionId in commitmentVersionIds)
                            {
                                await SaveCommitmentVersion(cancellationToken, dataLockStatusChangedEvent, commitmentVersionId).ConfigureAwait(false);
                                savedCommitmentVersions++;


                                // collection periods (and transaction types) are recorded per commitment version
                                // v1 does not record delivery periods so we only need to create a record per transaction type for current collection period
                                foreach (var transactionTypesAndPeriod in dataLockStatusChangedEvent.TransactionTypesAndPeriods)
                                {
                                    await SaveEventPeriods(cancellationToken, dataLockStatusChangedEvent, transactionTypesAndPeriod, isError, commitmentVersionId).ConfigureAwait(false);
                                    savedPeriods++;
                                }
                            }

                            if (priceEpisode.DataLockFailures?.Count > 0)
                            {
                                await SaveErrorCodes(dataLockStatusChangedEvent, priceEpisode.DataLockFailures, cancellationToken).ConfigureAwait(false);
                                savedErrors += priceEpisode.DataLockFailures.Count;
                            }
                        }

                        logger.LogDebug($"Saved DataLockStatusChanged event {dataLockStatusChangedEvent.EventId} for UKPRN {dataLockStatusChangedEvent.Ukprn}. Legacy events: {savedEvents}, commitment versions: {savedCommitmentVersions}, periods: {savedPeriods}, errors: {savedErrors}");
                    }

                    await dataLockEventWriter.Flush(cancellationToken).ConfigureAwait(false);
                    await dataLockEventCommitmentVersionWriter.Flush(cancellationToken).ConfigureAwait(false);
                    await dataLockEventPeriodWriter.Flush(cancellationToken).ConfigureAwait(false);
                    await dataLockEventErrorWriter.Flush(cancellationToken).ConfigureAwait(false);

                    scope.Complete();
                }
                catch (Exception e)
                {
                    logger.LogError($"Error saving batch of DataLockStatusChanged events. Error: {e.Message}", e);
                    throw;
                }
            }

            return batch.Count;

        }

        private async Task SaveErrorCodes(DataLockStatusChanged dataLockStatusChangedEvent, List<DataLockFailure> dataLockFailures, CancellationToken cancellationToken)
        {
            foreach (var dataLockFailure in dataLockFailures)
            {
                var dataLockEventError = new LegacyDataLockEventError
                {
                    DataLockEventId = dataLockStatusChangedEvent.EventId,
                    SystemDescription = dataLockFailure.DataLockError.ToString(),
                    ErrorCode = dataLockFailure.DataLockError.ToString()
                };

                logger.LogVerbose($"Saving DataLockEventError {dataLockFailure.DataLockError} for legacy DataLockEvent {dataLockStatusChangedEvent.EventId} for UKPRN {dataLockStatusChangedEvent.Ukprn}");

                await dataLockEventErrorWriter.Write(dataLockEventError, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task SaveEventPeriods(CancellationToken cancellationToken, DataLockStatusChanged dataLockStatusChangedEvent, KeyValuePair<TransactionType, List<EarningPeriod>> transactionTypesAndPeriod, bool isError, long commitmentVersionId)
        {
            var collectionPeriod = dataLockStatusChangedEvent.CollectionPeriod.Period;

            var dataLockEventPeriod = new LegacyDataLockEventPeriod
            {
                DataLockEventId = dataLockStatusChangedEvent.EventId,
                TransactionType = (int) transactionTypesAndPeriod.Key,
                CollectionPeriodYear = dataLockStatusChangedEvent.CollectionPeriod.AcademicYear,
                CollectionPeriodName = $"{dataLockStatusChangedEvent.CollectionPeriod.AcademicYear}-{collectionPeriod:D2}",
                CollectionPeriodMonth = (collectionPeriod < 6) ? collectionPeriod + 7 : collectionPeriod - 5,
                IsPayable = !isError,
                CommitmentVersion = commitmentVersionId.ToString()
            };

            logger.LogVerbose($"Saving DataLockEventPeriod {dataLockEventPeriod.CollectionPeriodName} for legacy DataLockEvent {dataLockStatusChangedEvent.EventId} for UKPRN {dataLockStatusChangedEvent.Ukprn}");

            await dataLockEventPeriodWriter.Write(dataLockEventPeriod, cancellationToken).ConfigureAwait(false);
        }

        private async Task SaveCommitmentVersion(CancellationToken cancellationToken, DataLockStatusChanged dataLockStatusChangedEvent, long commitmentVersionId)
        {
            var commitmentVersion = new LegacyDataLockEventCommitmentVersion
            {
                DataLockEventId = dataLockStatusChangedEvent.EventId,
                CommitmentVersion = commitmentVersionId.ToString(),
                //CommitmentEffectiveDate = 
                //CommitmentFrameworkCode = 
                //CommitmentNegotiatedPrice = 
                //CommitmentPathwayCode = 
                //CommitmentProgrammeType = 
                //CommitmentStandardCode = 
                //CommitmentStartDate =                             
            };

            logger.LogVerbose($"Saving DataLockEventCommitmentVersion {commitmentVersion.CommitmentVersion} for legacy DataLockEvent {dataLockStatusChangedEvent.EventId} for UKPRN {dataLockStatusChangedEvent.Ukprn}");

            await dataLockEventCommitmentVersionWriter.Write(commitmentVersion, cancellationToken).ConfigureAwait(false);
        }

        private static List<long> GetCommitmentVersions(bool isError, EarningPeriod priceEpisode)
        {
            // for error commitment versions can be multiple, for pass there is one.
            var commitmentVersionIds = new List<long>();

            if (isError)
            {
                commitmentVersionIds.AddRange(priceEpisode.DataLockFailures.Where(f => f.ApprenticeshipPriceEpisodeIds != null).SelectMany(f => f.ApprenticeshipPriceEpisodeIds).Distinct());
            }
            else
            {
                commitmentVersionIds.Add(priceEpisode.ApprenticeshipPriceEpisodeId.GetValueOrDefault(0));
            }

            return commitmentVersionIds;
        }

        private async Task<LegacyDataLockEvent> SaveDataLockEvent(CancellationToken cancellationToken, DataLockStatusChanged dataLockStatusChangedEvent, EarningPeriod priceEpisode)
        {
            var dataLockEvent = new LegacyDataLockEvent // commitment ID
            {
                AcademicYear = dataLockStatusChangedEvent.CollectionPeriod.AcademicYear.ToString(),
                UKPRN = dataLockStatusChangedEvent.Ukprn,
                DataLockEventId = dataLockStatusChangedEvent.EventId,
                EventSource = 1, // submission
                HasErrors = !(dataLockStatusChangedEvent is DataLockStatusChangedToPassed),
                ULN = dataLockStatusChangedEvent.Learner.Uln,
                Status = dataLockStatusChangedEvent is DataLockStatusChangedToPassed ? 3 : dataLockStatusChangedEvent is DataLockStatusChangedToFailed ? 1 : 2,
                ProcessDateTime = DateTime.UtcNow,
                LearnRefNumber = dataLockStatusChangedEvent.Learner.ReferenceNumber,
                IlrFrameworkCode = dataLockStatusChangedEvent.LearningAim.FrameworkCode,
                IlrPathwayCode = dataLockStatusChangedEvent.LearningAim.PathwayCode,
                IlrProgrammeType = dataLockStatusChangedEvent.LearningAim.ProgrammeType,
                IlrStandardCode = dataLockStatusChangedEvent.LearningAim.StandardCode,
                SubmittedDateTime = dataLockStatusChangedEvent.IlrSubmissionDateTime,

                PriceEpisodeIdentifier = priceEpisode.PriceEpisodeIdentifier,
                CommitmentId = priceEpisode.ApprenticeshipId.GetValueOrDefault(0),
                EmployerAccountId = priceEpisode.AccountId.GetValueOrDefault(0),

                //AimSeqNumber = 
                //IlrPriceEffectiveFromDate = price episode (earning via earning period PE ID)
                //IlrPriceEffectiveToDate = price episode (earning via earning period PE ID)
                //IlrEndpointAssessorPrice = price episode (earning via earning period PE ID)
                IlrFileName = dataLockStatusChangedEvent.IlrFileName,
                //IlrStartDate = price episode (earning via earning period PE ID)
                //IlrTrainingPrice = price episode (earning via earning period PE ID)
            };

            logger.LogVerbose($"Saving legacy DataLockEvent {dataLockStatusChangedEvent.EventId} for UKPRN {dataLockStatusChangedEvent.Ukprn}");

            await dataLockEventWriter.Write(dataLockEvent, cancellationToken).ConfigureAwait(false);
            return dataLockEvent;
        }
    }
}
