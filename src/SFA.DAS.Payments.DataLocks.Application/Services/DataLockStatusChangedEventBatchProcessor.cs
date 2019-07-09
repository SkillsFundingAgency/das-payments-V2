using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class DataLockStatusChangedEventBatchProcessor : IBatchProcessor<DataLockStatusChanged>
    {
        private readonly IBatchedDataCache<DataLockStatusChanged> cache;
        private readonly IPaymentLogger logger;
        private IMapper mapper;
        private readonly IBulkWriter<LegacyDataLockEvent> dataLockEventWriter;
        private readonly IBulkWriter<LegacyDataLockEventCommitmentVersion> dataLockEventCommitmentVersionWriter;
        private readonly IBulkWriter<LegacyDataLockEventError> dataLockEventErrorWriter;
        private readonly IBulkWriter<LegacyDataLockEventPeriod> dataLockEventPeriodWriter;

        public DataLockStatusChangedEventBatchProcessor(
            IBatchedDataCache<DataLockStatusChanged> cache,
            IPaymentLogger logger,
            IMapper mapper,
            IBulkWriter<LegacyDataLockEvent> dataLockEventWriter,
            IBulkWriter<LegacyDataLockEventCommitmentVersion> dataLockEventCommitmentVersionWriter,
            IBulkWriter<LegacyDataLockEventError> dataLockEventErrorWriter,
            IBulkWriter<LegacyDataLockEventPeriod> dataLockEventPeriodWriter)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
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
                        var errorCode = dataLockStatusChangedEvent is DataLockStatusChangedToFailed ? ((DataLockStatusChangedToFailed) dataLockStatusChangedEvent).ErrorCode : (DataLockErrorCode?) null;

                        // TODO: group by commitment ID

                        // v1 doesn't use delivery period, use price episode instead
                        var priceEpisodes = dataLockStatusChangedEvent.TransactionTypesAndPeriods.SelectMany(p => p.Value).GroupBy(p => p.PriceEpisodeIdentifier).Select(g => g.First()).ToList();

                        foreach (var priceEpisode in priceEpisodes)
                        {
                            var dataLockEvent = new LegacyDataLockEvent // commitment ID
                            {
                                AcademicYear = dataLockStatusChangedEvent.CollectionPeriod.AcademicYear.ToString(),
                                Ukprn = dataLockStatusChangedEvent.Ukprn,
                                DataLockEventId = dataLockStatusChangedEvent.EventId,
                                EventSource = 1, // submission
                                HasErrors = !(dataLockStatusChangedEvent is DataLockStatusChangedToPassed),
                                Uln = dataLockStatusChangedEvent.Learner.Uln,
                                Status = (dataLockStatusChangedEvent is DataLockStatusChangedToPassed) ? 3 : (dataLockStatusChangedEvent is DataLockStatusChangedToFailed) ? 1 : 2,
                                ProcessDateTime = DateTime.UtcNow,
                                LearnRefnumber = dataLockStatusChangedEvent.Learner.ReferenceNumber,
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
                                //IlrFileName = price episode (earning via earning period PE ID)
                                //IlrStartDate = price episode (earning via earning period PE ID)
                                //IlrTrainingPrice = price episode (earning via earning period PE ID)

                            };

                            await dataLockEventWriter.Write(dataLockEvent, cancellationToken).ConfigureAwait(false);



                            // for error commitment versions can be multiple, for pass there is one.
                            var commitmentVersionIds = new List<long>();

                            if (errorCode.HasValue)
                            {
                                commitmentVersionIds.AddRange(priceEpisode.DataLockFailures.SelectMany(f => f.ApprenticeshipPriceEpisodeIds).Distinct());
                            }
                            else
                            {
                                commitmentVersionIds.Add(priceEpisode.ApprenticeshipPriceEpisodeId.GetValueOrDefault(0));
                            }

                            foreach (var commitmentVersionId in commitmentVersionIds)
                            {
                                var commitmentVersions = new LegacyDataLockEventCommitmentVersion
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

                                await dataLockEventCommitmentVersionWriter.Write(commitmentVersions, cancellationToken).ConfigureAwait(false);
                            }

                            if (errorCode.HasValue)
                            {
                                var dataLockEventError = new LegacyDataLockEventError
                                {
                                    DataLockEventId = dataLockStatusChangedEvent.EventId,
                                    SystemDescription = errorCode.ToString(),
                                    ErrorCode = errorCode.ToString()
                                };

                                await dataLockEventErrorWriter.Write(dataLockEventError, cancellationToken).ConfigureAwait(false);
                            }



                            foreach (var transactionTypesAndPeriod in dataLockStatusChangedEvent.TransactionTypesAndPeriods)
                            {
                                var period = dataLockStatusChangedEvent.CollectionPeriod.Period;

                                var dataLockEventPeriod = new LegacyDataLockEventPeriod
                                {
                                    DataLockEventId = dataLockStatusChangedEvent.EventId,
                                    TransactionType = (int)transactionTypesAndPeriod.Key,
                                    CollectionPeriodYear = dataLockStatusChangedEvent.CollectionPeriod.AcademicYear,
                                    CollectionPeriodName = $"{dataLockStatusChangedEvent.CollectionPeriod.AcademicYear}-{period:D2}" ,
                                    CollectionPeriodMonth = (period < 6) ? period + 7 : period - 5,
                                    IsPayable = !errorCode.HasValue

                                    //CommitmentVersion = 
                                };
                                await dataLockEventPeriodWriter.Write(dataLockEventPeriod, cancellationToken).ConfigureAwait(false);
                            }


                        }



                        logger.LogVerbose($"Saving {typeof(T).Name} to table: {dataLockStatusChangedEvent}");
                        await bulkWriter.Write(dataLockStatusChangedEvent, cancellationToken).ConfigureAwait(false);
                    }

                    await bulkWriter.Flush(cancellationToken).ConfigureAwait(false);
                    scope.Complete();
                }
                catch (Exception e)
                {
                    logger.LogError($"Error performing bulk copy for model type: {typeof(T).Name}. Error: {e.Message}", e);
                    throw;
                }
            }

            return batch.Count;

        }
    }
}
