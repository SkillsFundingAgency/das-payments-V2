using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
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
        private readonly IApprenticeshipRepository apprenticeshipRepository;
        private IDictionary<long, ApprenticeshipModel> apprenticeshipCache;

        public DataLockStatusChangedEventBatchProcessor(
            IBatchedDataCache<DataLockStatusChanged> cache,
            IPaymentLogger logger,
            IBulkWriter<LegacyDataLockEvent> dataLockEventWriter,
            IBulkWriter<LegacyDataLockEventCommitmentVersion> dataLockEventCommitmentVersionWriter,
            IBulkWriter<LegacyDataLockEventError> dataLockEventErrorWriter,
            IBulkWriter<LegacyDataLockEventPeriod> dataLockEventPeriodWriter, 
            IApprenticeshipRepository apprenticeshipRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.dataLockEventWriter = dataLockEventWriter;
            this.dataLockEventCommitmentVersionWriter = dataLockEventCommitmentVersionWriter;
            this.dataLockEventErrorWriter = dataLockEventErrorWriter;
            this.dataLockEventPeriodWriter = dataLockEventPeriodWriter;
            this.apprenticeshipRepository = apprenticeshipRepository;
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
                await PopulateApprenticeshipCache(batch, cancellationToken).ConfigureAwait(false);

                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var dataLockStatusChangedEvent in batch)
                    {
                        int savedEvents = 0, savedCommitmentVersions = 0, savedPeriods = 0, savedErrors = 0;
                        var isError = dataLockStatusChangedEvent is DataLockStatusChangedToFailed || dataLockStatusChangedEvent is DataLockFailureChanged;
                        var flatPeriodList = dataLockStatusChangedEvent.TransactionTypesAndPeriods.SelectMany(tp => tp.Value).ToList();

                        // there may be multiple commitment IDs when employer changes or separate errors against two employers
                        var apprenticeshipIds = flatPeriodList.Select(p => p.ApprenticeshipId)
                            .Concat(flatPeriodList.SelectMany(p => p.DataLockFailures.Select(f => f.ApprenticeshipId)))
                            .Distinct()
                            .ToList();

                        foreach (var apprenticeshipId in apprenticeshipIds)
                        {
                            // v1 doesn't use delivery period, get one earning period for each price episode
                            var earningPeriodsByPriceEpisode = flatPeriodList
                                .GroupBy(p => p.PriceEpisodeIdentifier)
                                .Select(g => g.FirstOrDefault(p => p.ApprenticeshipId == apprenticeshipId || p.DataLockFailures.Any(f => f.ApprenticeshipId == apprenticeshipId)))
                                .Where(g => g != null)
                                .ToList();

                            foreach (var earningPeriod in earningPeriodsByPriceEpisode)
                            {
                                // only records null apprenticeship when DLOCK 01 & 02
                                if (!apprenticeshipId.HasValue)
                                {
                                    if (earningPeriod.DataLockFailures.Any(f => f.ApprenticeshipId.HasValue))
                                        continue;
                                }

                                await SaveDataLockEvent(cancellationToken, dataLockStatusChangedEvent, earningPeriod).ConfigureAwait(false);
                                savedEvents++;

                                if (apprenticeshipId.HasValue)
                                {
                                    if (isError)
                                    {
                                        var writtentVersions = new HashSet<(long apprenticeshipId, long apprenticeshipPriceEpisodeId)>();

                                        foreach (var dataLockFailure in earningPeriod.DataLockFailures.Where(f => f.ApprenticeshipId == apprenticeshipId))
                                        {
                                            foreach (var apprenticeshipPriceEpisodeId in dataLockFailure.ApprenticeshipPriceEpisodeIds)
                                            {
                                                // there are multiple errors recorded for the same apprenticeship episode
                                                if (writtentVersions.Contains((apprenticeshipId.Value, apprenticeshipPriceEpisodeId)))
                                                    continue;

                                                savedPeriods += await SaveCommitmentVersionAndPeriods(dataLockStatusChangedEvent, dataLockFailure.ApprenticeshipId.Value, apprenticeshipPriceEpisodeId, isError, cancellationToken);
                                                savedCommitmentVersions++;

                                                writtentVersions.Add((apprenticeshipId.Value, apprenticeshipPriceEpisodeId));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        savedPeriods += await SaveCommitmentVersionAndPeriods(dataLockStatusChangedEvent, earningPeriod.ApprenticeshipId.Value, earningPeriod.ApprenticeshipPriceEpisodeId.Value, isError, cancellationToken);
                                        savedCommitmentVersions++;
                                    }
                                }

                                if (earningPeriod.DataLockFailures?.Count > 0)
                                {
                                    await SaveErrorCodes(dataLockStatusChangedEvent, earningPeriod.DataLockFailures, cancellationToken).ConfigureAwait(false);
                                    savedErrors += earningPeriod.DataLockFailures.Count;
                                }
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
            }
            catch (Exception e)
            {
                logger.LogError($"Error saving batch of DataLockStatusChanged events. Error: {e.Message}", e);
                throw;
            }

            return batch.Count;
        }

        private async Task<int> SaveCommitmentVersionAndPeriods(DataLockStatusChanged dataLockStatusChangedEvent, long apprenticeshipId, long apprenticeshipPriceEpisodeId, bool isError, CancellationToken cancellationToken)
        {
            int savedPeriods = 0;
            await SaveCommitmentVersion(cancellationToken, dataLockStatusChangedEvent, apprenticeshipId, apprenticeshipPriceEpisodeId).ConfigureAwait(false);

            // collection periods (and transaction types) are recorded per commitment version
            // v1 does not record delivery periods so we only need to create a record per transaction type for current collection period
            foreach (var transactionTypesAndPeriod in dataLockStatusChangedEvent.TransactionTypesAndPeriods)
            {
                await SaveEventPeriods(cancellationToken, dataLockStatusChangedEvent, transactionTypesAndPeriod, isError, apprenticeshipPriceEpisodeId).ConfigureAwait(false);
                savedPeriods++;
            }

            return savedPeriods;
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
                TransactionTypesFlag = GetTransactionTypeFlag(transactionTypesAndPeriod.Key),
                CollectionPeriodYear = dataLockStatusChangedEvent.CollectionPeriod.AcademicYear,
                CollectionPeriodName = $"{dataLockStatusChangedEvent.CollectionPeriod.AcademicYear}-{collectionPeriod:D2}",
                CollectionPeriodMonth = (collectionPeriod < 6) ? collectionPeriod + 7 : collectionPeriod - 5,
                IsPayable = !isError,
                CommitmentVersion = commitmentVersionId.ToString()
            };

            logger.LogVerbose($"Saving DataLockEventPeriod {dataLockEventPeriod.CollectionPeriodName} for legacy DataLockEvent {dataLockStatusChangedEvent.EventId} for UKPRN {dataLockStatusChangedEvent.Ukprn}");

            await dataLockEventPeriodWriter.Write(dataLockEventPeriod, cancellationToken).ConfigureAwait(false);
        }

        private int GetTransactionTypeFlag(TransactionType transactionType)
        {
            switch (transactionType)
            {
                case TransactionType.Learning:
                case TransactionType.OnProgramme16To18FrameworkUplift:
                case TransactionType.OnProgrammeMathsAndEnglish:
                case TransactionType.BalancingMathsAndEnglish:
                case TransactionType.LearningSupport:
                    return 1;

                case TransactionType.First16To18EmployerIncentive:
                case TransactionType.First16To18ProviderIncentive:
                case TransactionType.FirstDisadvantagePayment:
                    return 2;

                case TransactionType.Second16To18EmployerIncentive:
                case TransactionType.Second16To18ProviderIncentive:
                case TransactionType.SecondDisadvantagePayment:
                    return 3;

                case TransactionType.Completion:
                case TransactionType.Balancing:
                case TransactionType.Completion16To18FrameworkUplift:
                case TransactionType.Balancing16To18FrameworkUplift:
                    return 4;

                case TransactionType.CareLeaverApprenticePayment:
                    return 5;

                default:
                    throw new ArgumentException($"Transaction Type {transactionType} not supported.", nameof(transactionType));
            }
        }

        private async Task SaveCommitmentVersion(CancellationToken cancellationToken, DataLockStatusChanged dataLockStatusChangedEvent, long apprenticeshipId, long apprenticeshipPriceEpisodeId)
        {
            var apprenticeship = apprenticeshipCache[apprenticeshipId];
            var apprenticeshipPriceEpisode = apprenticeship.ApprenticeshipPriceEpisodes.Single(e => e.Id == apprenticeshipPriceEpisodeId);

            var commitmentVersion = new LegacyDataLockEventCommitmentVersion
            {
                DataLockEventId = dataLockStatusChangedEvent.EventId,
                CommitmentVersion = $"{apprenticeship.Id}-{apprenticeshipPriceEpisode.Id}",
                CommitmentEffectiveDate = apprenticeshipPriceEpisode.StartDate,
                CommitmentFrameworkCode = apprenticeship.FrameworkCode,
                CommitmentNegotiatedPrice = apprenticeshipPriceEpisode.Cost,
                CommitmentPathwayCode = apprenticeship.PathwayCode,
                CommitmentProgrammeType = apprenticeship.ProgrammeType,
                CommitmentStandardCode = apprenticeship.StandardCode,
                CommitmentStartDate = apprenticeship.EstimatedStartDate
            };

            logger.LogVerbose($"Saving DataLockEventCommitmentVersion {commitmentVersion.CommitmentVersion} for legacy DataLockEvent {dataLockStatusChangedEvent.EventId} for UKPRN {dataLockStatusChangedEvent.Ukprn}");

            await dataLockEventCommitmentVersionWriter.Write(commitmentVersion, cancellationToken).ConfigureAwait(false);
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
                CommitmentId = priceEpisode.ApprenticeshipId ?? 0,
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

        private async Task PopulateApprenticeshipCache(List<DataLockStatusChanged> statusChangeEvents, CancellationToken cancellationToken)
        {
            var allPeriods = statusChangeEvents.SelectMany(e => e.TransactionTypesAndPeriods.SelectMany(p => p.Value)).ToList();
            var allApprenticeshipIds = allPeriods.Select(p => p.ApprenticeshipId)
                .Concat(allPeriods.Where(p => p.DataLockFailures != null).SelectMany(p => p.DataLockFailures.Select(f => f.ApprenticeshipId)))                
                .Distinct()
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            var apprenticeshipModels = await apprenticeshipRepository.Get(allApprenticeshipIds, cancellationToken).ConfigureAwait(false);
            apprenticeshipCache = apprenticeshipModels.ToDictionary(m => m.Id, m => m);
        }
    }
}
