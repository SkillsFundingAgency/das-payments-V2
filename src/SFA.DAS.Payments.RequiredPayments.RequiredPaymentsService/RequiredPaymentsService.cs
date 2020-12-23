using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Application;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService
{
    [StatePersistence(StatePersistence.Volatile)]
    public class RequiredPaymentsService : Actor, IRequiredPaymentsService
    {
        private ReliableCollectionCache<PaymentHistoryEntity[]> paymentHistoryCache;
        private ReliableCollectionCache<CollectionPeriod> collectionPeriodCache;
        private readonly IPaymentLogger paymentLogger;
        private readonly ApprenticeshipKey apprenticeshipKey;
        private readonly string apprenticeshipKeyString;
        private readonly Func<IPaymentHistoryRepository> paymentHistoryRepositoryFactory;
        private readonly IApprenticeshipContractType2EarningsEventProcessor contractType2EarningsEventProcessor;
        private readonly IApprenticeshipAct1RedundancyEarningsEventProcessor act1RedundancyEarningsEventProcessor;
        private readonly IFunctionalSkillEarningsEventProcessor functionalSkillEarningsEventProcessor;
        private readonly IPayableEarningEventProcessor payableEarningEventProcessor;
        private readonly IRefundRemovedLearningAimProcessor refundRemovedLearningAimProcessor;
        readonly ITelemetry telemetry;
        private readonly string logSafeApprenticeshipKeyString;
        


        public RequiredPaymentsService(ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger,
            IApprenticeshipKeyService apprenticeshipKeyService,
            Func<IPaymentHistoryRepository> paymentHistoryRepositoryFactory,
            IApprenticeshipContractType2EarningsEventProcessor contractType2EarningsEventProcessor,
            IApprenticeshipAct1RedundancyEarningsEventProcessor act1RedundancyEarningsEventProcessor, 
            IFunctionalSkillEarningsEventProcessor functionalSkillEarningsEventProcessor,
            IPayableEarningEventProcessor payableEarningEventProcessor,
            IRefundRemovedLearningAimProcessor refundRemovedLearningAimProcessor,
            ITelemetry telemetry)
            : base(actorService, actorId)
        {
            this.paymentLogger = paymentLogger;
            this.paymentHistoryRepositoryFactory = paymentHistoryRepositoryFactory;
            this.contractType2EarningsEventProcessor = contractType2EarningsEventProcessor;
            this.act1RedundancyEarningsEventProcessor = act1RedundancyEarningsEventProcessor;
            this.functionalSkillEarningsEventProcessor = functionalSkillEarningsEventProcessor;
            this.payableEarningEventProcessor = payableEarningEventProcessor;
            this.refundRemovedLearningAimProcessor = refundRemovedLearningAimProcessor;
            this.telemetry = telemetry;
            apprenticeshipKeyString = actorId.GetStringId();
            apprenticeshipKey = apprenticeshipKeyService.ParseApprenticeshipKey(apprenticeshipKeyString);
            logSafeApprenticeshipKeyString = CreateLogSafeApprenticeshipKeyString(apprenticeshipKey);
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleApprenticeship2ContractTypeEarningsEvent(ApprenticeshipContractType2EarningEvent earningEvent, CancellationToken cancellationToken)
        {
            paymentLogger.LogVerbose($"Handling ApprenticeshipContractType2EarningEvent for jobId:{earningEvent.JobId} with apprenticeship key based on {logSafeApprenticeshipKeyString}");
            
            using (var operation = telemetry.StartOperation("RequiredPaymentsService.HandleApprenticeship2ContractTypeEarningsEvent", earningEvent.EventId.ToString()))
            {
                var stopwatch = Stopwatch.StartNew();
                await ResetPaymentHistoryCacheIfDifferentCollectionPeriod(earningEvent.CollectionPeriod)
                    .ConfigureAwait(false);

                await Initialise(earningEvent.CollectionPeriod.Period).ConfigureAwait(false);
                var requiredPaymentEvents = await contractType2EarningsEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCache, cancellationToken).ConfigureAwait(false);
                Log(requiredPaymentEvents);
                telemetry.TrackDuration("RequiredPaymentsService.HandleApprenticeship2ContractTypeEarningsEvent", stopwatch, earningEvent);
                telemetry.StopOperation(operation);
                return requiredPaymentEvents;
            }
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleAct2RedundancyEarningEvent(ApprenticeshipContractType2RedundancyEarningEvent earningEvent,
            CancellationToken cancellationToken)
        {
            paymentLogger.LogVerbose($"Handling ApprenticeshipContractType2RedundancyEarningEvent for jobId:{earningEvent.JobId} with apprenticeship key based on {logSafeApprenticeshipKeyString}");

            using (var operation = telemetry.StartOperation("RequiredPaymentsService.HandleAct2RedundancyEarningEvent", earningEvent.EventId.ToString()))
            {
                var stopwatch = Stopwatch.StartNew();
                await ResetPaymentHistoryCacheIfDifferentCollectionPeriod(earningEvent.CollectionPeriod)
                    .ConfigureAwait(false);

                await Initialise(earningEvent.CollectionPeriod.Period).ConfigureAwait(false);
                var requiredPaymentEvents = await contractType2EarningsEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCache, cancellationToken).ConfigureAwait(false);
                Log(requiredPaymentEvents);
                telemetry.TrackDuration("RequiredPaymentsService.HandleAct2RedundancyEarningEvent", stopwatch, earningEvent);
                telemetry.StopOperation(operation);
                return requiredPaymentEvents;
            }

        }


        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleFunctionalSkillEarningsEvent(FunctionalSkillEarningsEvent earningEvent, CancellationToken cancellationToken)
        {
            paymentLogger.LogVerbose($"Handling FunctionalSkillEarningsEvent for jobId:{earningEvent.JobId} with apprenticeship key based on {logSafeApprenticeshipKeyString}");

            using (var operation = telemetry.StartOperation())
            {
                var stopwatch = Stopwatch.StartNew();
                await ResetPaymentHistoryCacheIfDifferentCollectionPeriod(earningEvent.CollectionPeriod)
                    .ConfigureAwait(false);

                await Initialise(earningEvent.CollectionPeriod.Period).ConfigureAwait(false);
                var requiredPaymentEvents = await functionalSkillEarningsEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCache, cancellationToken).ConfigureAwait(false);
                Log(requiredPaymentEvents);
                telemetry.TrackDuration("RequiredPaymentsService.HandleFunctionalSkillEarningsEvent", stopwatch, earningEvent);
                telemetry.StopOperation(operation);
                return requiredPaymentEvents;
            }
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandlePayableFunctionalSkillEarningsEvent(PayableFunctionalSkillEarningEvent earningEvent, CancellationToken cancellationToken)
        {
            paymentLogger.LogVerbose($"Handling PayableFunctionalSkillEarningsEvent for jobId:{earningEvent.JobId} with apprenticeship key based on {logSafeApprenticeshipKeyString}");

            using (var operation = telemetry.StartOperation("RequiredPaymentsService.HandleFunctionalSkillEarningsEvent", earningEvent.EventId.ToString()))
            {
                var stopwatch = Stopwatch.StartNew();
                await ResetPaymentHistoryCacheIfDifferentCollectionPeriod(earningEvent.CollectionPeriod)
                    .ConfigureAwait(false);

                await Initialise(earningEvent.CollectionPeriod.Period).ConfigureAwait(false);
                var requiredPaymentEvents = await functionalSkillEarningsEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCache, cancellationToken).ConfigureAwait(false);
                Log(requiredPaymentEvents);
                telemetry.TrackDuration("RequiredPaymentsService.HandleFunctionalSkillEarningsEvent", stopwatch, earningEvent);
                telemetry.StopOperation(operation);
                return requiredPaymentEvents;
            }
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleAct1RedundancyEarningsEvent(ApprenticeshipContractType1RedundancyEarningEvent earningEvent,
            CancellationToken cancellationToken)
        {
            paymentLogger.LogVerbose($"Handling ApprenticeshipContractType1RedundancyEarningEvent for jobId:{earningEvent.JobId} with apprenticeship key based on {logSafeApprenticeshipKeyString}");

            using (var operation = telemetry.StartOperation("RequiredPaymentsService.ApprenticeshipContractType1RedundancyEarningEvent", earningEvent.EventId.ToString()))
            {
                var stopwatch = Stopwatch.StartNew();
                await ResetPaymentHistoryCacheIfDifferentCollectionPeriod(earningEvent.CollectionPeriod)
                    .ConfigureAwait(false);

                await Initialise(earningEvent.CollectionPeriod.Period).ConfigureAwait(false);
                var requiredPaymentEvents = await act1RedundancyEarningsEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCache, cancellationToken).ConfigureAwait(false);
                Log(requiredPaymentEvents);
                telemetry.TrackDuration("RequiredPaymentsService.ApprenticeshipContractType1RedundancyEarningEvent", stopwatch, earningEvent);
                telemetry.StopOperation(operation);
                return requiredPaymentEvents;
            }
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandlePayableEarningEvent(PayableEarningEvent earningEvent, CancellationToken cancellationToken)
        {
            paymentLogger.LogVerbose($"Handling PayableEarningEvent for jobId:{earningEvent.JobId} with apprenticeship key based on {logSafeApprenticeshipKeyString}");
            try
            {
                using (var operation = telemetry.StartOperation("RequiredPaymentsService.HandlePayableEarningEvent", earningEvent.EventId.ToString()))
                {
                    var stopwatch = Stopwatch.StartNew();
                    await ResetPaymentHistoryCacheIfDifferentCollectionPeriod(earningEvent.CollectionPeriod)
                        .ConfigureAwait(false);

                    await Initialise(earningEvent.CollectionPeriod.Period).ConfigureAwait(false);
                    var requiredPaymentEvents = await payableEarningEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCache, cancellationToken).ConfigureAwait(false);
                    Log(requiredPaymentEvents);
                    telemetry.TrackDuration("RequiredPaymentsService.HandlePayableEarningEvent", stopwatch, earningEvent);
                    telemetry.StopOperation(operation);
                    return requiredPaymentEvents;
                }
            }
            catch (Exception e)
            {
                paymentLogger.LogError($"Error handling payable earning. Error: {e.Message}");
                throw;
            }
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> RefundRemovedLearningAim(IdentifiedRemovedLearningAim removedLearningAim, CancellationToken cancellationToken)
        {
            paymentLogger.LogDebug($"Handling identified removed learning aim for jobId:{removedLearningAim.JobId} with apprenticeship key based on {logSafeApprenticeshipKeyString}");
            using (var operation = telemetry.StartOperation("RequiredPaymentsService.RefundRemovedLearningAim", removedLearningAim.EventId.ToString()))
            {
                var stopwatch = Stopwatch.StartNew();
                await ResetPaymentHistoryCacheIfDifferentCollectionPeriod(removedLearningAim.CollectionPeriod)
                    .ConfigureAwait(false);

                await Initialise(removedLearningAim.CollectionPeriod.Period).ConfigureAwait(false);
                var requiredPaymentEvents = await refundRemovedLearningAimProcessor.RefundLearningAim(removedLearningAim, paymentHistoryCache, cancellationToken).ConfigureAwait(false);
                Log(requiredPaymentEvents);
                // removed aim would will not receive a command to clear cache
                await Reset().ConfigureAwait(false);
                telemetry.TrackDuration("RequiredPaymentsService.RefundRemovedLearningAim", stopwatch, removedLearningAim);
                telemetry.StopOperation(operation);
                return requiredPaymentEvents;
            }
        }

        private void Log(ReadOnlyCollection<PeriodisedRequiredPaymentEvent> requiredPaymentEvents)
        {
            var stats = requiredPaymentEvents.GroupBy(p => p.GetType()).Select(group => $"{group.Key.Name}: {group.Count()}");
            paymentLogger.LogDebug($"Created {requiredPaymentEvents.Count} required payment events for jobId:{requiredPaymentEvents.FirstOrDefault()?.JobId} with apprenticeship key based on {apprenticeshipKeyString}. {string.Join(", ", stats)}");
        }

        protected override async Task OnActivateAsync()
        {
            using (var operation = telemetry.StartOperation("RequiredPaymentsService.RefundRemovedLearningAim",$"{apprenticeshipKeyString}_{Guid.NewGuid():N}"))
            {
                var stopwatch = Stopwatch.StartNew();
                //TODO: why are we still doing this?  We are supposed to be resolving this from the container.
                paymentHistoryCache = new ReliableCollectionCache<PaymentHistoryEntity[]>(StateManager);
                collectionPeriodCache = new ReliableCollectionCache<CollectionPeriod>(StateManager);

                //await Initialise().ConfigureAwait(false);

                await base.OnActivateAsync().ConfigureAwait(false);
                TrackInfrastructureEvent("RequiredPaymentsService.OnActivateAsync", stopwatch);
                telemetry.StopOperation(operation);
            }
        }

        private void TrackInfrastructureEvent(string eventName, Stopwatch stopwatch)
        {
            telemetry.TrackEvent(eventName,
                new Dictionary<string, string>
                {
                    { "ApprenticeshipKey", logSafeApprenticeshipKeyString},
                    { TelemetryKeys.LearnerRef, apprenticeshipKey.LearnerReferenceNumber},
                    { TelemetryKeys.AcademicYear, apprenticeshipKey.AcademicYear.ToString()},
                },
                new Dictionary<string, double>
                {
                    { TelemetryKeys.Duration, stopwatch.ElapsedMilliseconds }
                });
        }

        private async Task ResetPaymentHistoryCacheIfDifferentCollectionPeriod(CollectionPeriod collectionPeriod)
        {
            paymentLogger.LogVerbose($"Checking if collection period is new collection. Period: {collectionPeriod.Period}, Year: {collectionPeriod.AcademicYear}");
            var isNewCollection = await IsNewCollection(collectionPeriod).ConfigureAwait(false);
            if (!isNewCollection)
            {
                paymentLogger.LogVerbose("Is not new collection period.");
                return;
            }

            paymentLogger.LogDebug("Is new collection period, resetting the payment history cache.");
            await Reset().ConfigureAwait(false);
            await StoreCollectionPeriod(collectionPeriod).ConfigureAwait(false);
            paymentLogger.LogInfo($"Finished storing the new collection period '{collectionPeriod.Period}-{collectionPeriod.AcademicYear}' with apprenticeship key based on {logSafeApprenticeshipKeyString}");
        }

        private async Task<bool> IsNewCollection(CollectionPeriod collectionPeriod)
        {
            var cacheItem = await collectionPeriodCache.TryGet("CollectionPeriod").ConfigureAwait(false);
            if (!cacheItem.HasValue)
                return true;
            var cachedCollectionPeriod = cacheItem.Value;
            return collectionPeriod.AcademicYear != cachedCollectionPeriod.AcademicYear
                   || collectionPeriod.Period != cachedCollectionPeriod.Period;
        }

        private async Task StoreCollectionPeriod(CollectionPeriod collectionPeriod)
        {
            await collectionPeriodCache.AddOrReplace("CollectionPeriod", collectionPeriod).ConfigureAwait(false);
        }

        public async Task Initialise(byte currentCollectionPeriod)
        {
            if (await StateManager.ContainsStateAsync(CacheKeys.InitialisedKey).ConfigureAwait(false))
            {
                paymentLogger.LogVerbose($"Actor already initialised with apprenticeship key based on {logSafeApprenticeshipKeyString}");
                return;
            }
            var stopwatch = Stopwatch.StartNew();
            paymentLogger.LogDebug($"Initialising actor with apprenticeship key based on {logSafeApprenticeshipKeyString}");
            using (var paymentHistoryRepository = paymentHistoryRepositoryFactory())
            {
                var paymentHistory = await paymentHistoryRepository.GetPaymentHistory(apprenticeshipKey, currentCollectionPeriod, CancellationToken.None).ConfigureAwait(false);
                await paymentHistoryCache.AddOrReplace(CacheKeys.PaymentHistoryKey, paymentHistory.ToArray(), CancellationToken.None).ConfigureAwait(false);
            }
            await StateManager.TryAddStateAsync(CacheKeys.InitialisedKey, true).ConfigureAwait(false);
            paymentLogger.LogInfo($"Initialised actor with apprenticeship key based on {logSafeApprenticeshipKeyString}");
            stopwatch.Stop();
            TrackInfrastructureEvent("RequiredPaymentsService.Initialise", stopwatch);
        }

        public async Task Reset()
        {
            paymentLogger.LogDebug($"Resetting actor with apprenticeship key based on {logSafeApprenticeshipKeyString}");
            await StateManager.TryRemoveStateAsync(CacheKeys.InitialisedKey, CancellationToken.None).ConfigureAwait(false);
            //TODO: why are we not removing the history here?
            paymentLogger.LogInfo($"Finished resetting actor with apprenticeship key based on {logSafeApprenticeshipKeyString}");
        }

        private string CreateLogSafeApprenticeshipKeyString(ApprenticeshipKey key)
        {
            return $"learnerRef:{key.LearnerReferenceNumber}, frameworkCode:{key.FrameworkCode}, " +
                                     $"pathwayCode:{key.PathwayCode}, programmeType:{key.ProgrammeType}, " +
                                     $"standardCode:{key.StandardCode}, learningAimReference:{key.LearnAimRef}, " +
                                     $"academicYear:{key.AcademicYear}, contractType:{key.ContractType}";
        }
    }
}