using System;
using System.Collections;
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

        private readonly IPaymentLogger paymentLogger;
        private readonly ApprenticeshipKey apprenticeshipKey;
        private readonly string apprenticeshipKeyString;
        private readonly Func<IPaymentHistoryRepository> paymentHistoryRepositoryFactory;
        private readonly IApprenticeshipContractType2EarningsEventProcessor contractType2EarningsEventProcessor;
        private readonly IFunctionalSkillEarningsEventProcessor functionalSkillEarningsEventProcessor;
        private readonly IPayableEarningEventProcessor payableEarningEventProcessor;
        private readonly IRefundRemovedLearningAimProcessor refundRemovedLearningAimProcessor;
        readonly ITelemetry telemetry;


        public RequiredPaymentsService(ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger,
            IApprenticeshipKeyService apprenticeshipKeyService,
            Func<IPaymentHistoryRepository> paymentHistoryRepositoryFactory,
            IPaymentKeyService paymentKeyService,
            IApprenticeshipContractType2EarningsEventProcessor contractType2EarningsEventProcessor,
            IFunctionalSkillEarningsEventProcessor functionalSkillEarningsEventProcessor,
            IPayableEarningEventProcessor payableEarningEventProcessor,
            IRefundRemovedLearningAimProcessor refundRemovedLearningAimProcessor,
            ITelemetry telemetry)
            : base(actorService, actorId)
        {
            this.paymentLogger = paymentLogger;
            this.paymentHistoryRepositoryFactory = paymentHistoryRepositoryFactory;
            this.contractType2EarningsEventProcessor = contractType2EarningsEventProcessor;
            this.functionalSkillEarningsEventProcessor = functionalSkillEarningsEventProcessor;
            this.payableEarningEventProcessor = payableEarningEventProcessor;
            this.refundRemovedLearningAimProcessor = refundRemovedLearningAimProcessor;
            this.telemetry = telemetry;
            apprenticeshipKeyString = actorId.GetStringId();
            apprenticeshipKey = apprenticeshipKeyService.ParseApprenticeshipKey(apprenticeshipKeyString);
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleApprenticeship2ContractTypeEarningsEvent(ApprenticeshipContractType2EarningEvent earningEvent, CancellationToken cancellationToken)
        {
            paymentLogger.LogVerbose($"Handling ApprenticeshipContractType2EarningEvent for {apprenticeshipKeyString}");

            using (var operation = telemetry.StartOperation("RequiredPaymentsService.HandleApprenticeship2ContractTypeEarningsEvent", earningEvent.EventId.ToString()))
            {
                var stopwatch = StartStopwatch();
                await Initialise().ConfigureAwait(false);
                var requiredPaymentEvents = await contractType2EarningsEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCache, cancellationToken).ConfigureAwait(false);
                Log(requiredPaymentEvents);
                telemetry.TrackDuration("RequiredPaymentsService.HandleApprenticeship2ContractTypeEarningsEvent", stopwatch, earningEvent);
                telemetry.StopOperation(operation);
                return requiredPaymentEvents;
            }
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleFunctionalSkillEarningsEvent(FunctionalSkillEarningsEvent earningEvent, CancellationToken cancellationToken)
        {
            paymentLogger.LogVerbose($"Handling FunctionalSkillEarningsEvent for {apprenticeshipKeyString}");

            using (var operation = telemetry.StartOperation("RequiredPaymentsService.HandleFunctionalSkillEarningsEvent", earningEvent.EventId.ToString()))
            {
                var stopwatch = StartStopwatch();
                await Initialise().ConfigureAwait(false);
                var requiredPaymentEvents = await functionalSkillEarningsEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCache, cancellationToken).ConfigureAwait(false);
                Log(requiredPaymentEvents);
                telemetry.TrackDuration("RequiredPaymentsService.HandleFunctionalSkillEarningsEvent", stopwatch, earningEvent);
                telemetry.StopOperation(operation);
                return requiredPaymentEvents;
            }
        }

        private Stopwatch StartStopwatch()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            return stopwatch;
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandlePayableEarningEvent(PayableEarningEvent earningEvent, CancellationToken cancellationToken)
        {
            paymentLogger.LogVerbose($"Handling PayableEarningEvent for {apprenticeshipKeyString}");
            try
            {
                using (var operation = telemetry.StartOperation("RequiredPaymentsService.HandlePayableEarningEvent", earningEvent.EventId.ToString()))
                {
                    var stopwatch = StartStopwatch();
                    await Initialise().ConfigureAwait(false);
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
            paymentLogger.LogDebug($"Handling identified removed learning aim for {apprenticeshipKeyString}.");
            using (var operation = telemetry.StartOperation("RequiredPaymentsService.RefundRemovedLearningAim", removedLearningAim.EventId.ToString()))
            {
                var stopwatch = StartStopwatch();
                await Initialise().ConfigureAwait(false);
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
            paymentLogger.LogDebug($"Created {requiredPaymentEvents.Count} required payment events for {apprenticeshipKeyString}. {string.Join(", ", stats)}");
        }

        protected override async Task OnActivateAsync()
        {
            using (var operation = telemetry.StartOperation("RequiredPaymentsService.RefundRemovedLearningAim",$"{apprenticeshipKeyString}_{Guid.NewGuid():N}"))
            {
                var stopwatch = StartStopwatch();
                //TODO: why are we still doing this?  We are supposed to be resolving this from the container.
                paymentHistoryCache = new ReliableCollectionCache<PaymentHistoryEntity[]>(StateManager);

                await Initialise().ConfigureAwait(false);

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
                    { "ActorId", apprenticeshipKeyString},
                    { TelemetryKeys.LearnerRef, apprenticeshipKey.LearnerReferenceNumber},
                    { TelemetryKeys.AcademicYear, apprenticeshipKey.AcademicYear.ToString()},
                    { TelemetryKeys.Ukprn, apprenticeshipKey.Ukprn.ToString()},
                },
                new Dictionary<string, double>
                {
                    { TelemetryKeys.Duration, stopwatch.ElapsedMilliseconds }
                });
        }

        public async Task Initialise()
        {
            if (await StateManager.ContainsStateAsync(CacheKeys.InitialisedKey).ConfigureAwait(false))
            {
                paymentLogger.LogVerbose($"Actor already initialised for apprenticeship {apprenticeshipKeyString}");
                return;
            }
            var stopwatch = StartStopwatch();
            paymentLogger.LogDebug($"Initialising actor for apprenticeship {apprenticeshipKeyString}");
            using (var paymentHistoryRepository = paymentHistoryRepositoryFactory())
            {
                var paymentHistory = await paymentHistoryRepository.GetPaymentHistory(apprenticeshipKey, CancellationToken.None).ConfigureAwait(false);
                await paymentHistoryCache.AddOrReplace(CacheKeys.PaymentHistoryKey, paymentHistory.ToArray(), CancellationToken.None).ConfigureAwait(false);
            }
            await StateManager.TryAddStateAsync(CacheKeys.InitialisedKey, true).ConfigureAwait(false);
            paymentLogger.LogInfo($"Initialised actor for apprenticeship {apprenticeshipKeyString}");
            stopwatch.Stop();
            TrackInfrastructureEvent("RequiredPaymentsService.Initialise", stopwatch);
        }

        public async Task Reset()
        {
            paymentLogger.LogDebug($"Resetting actor for apprenticeship {apprenticeshipKeyString}");
            await StateManager.TryRemoveStateAsync(CacheKeys.InitialisedKey, CancellationToken.None).ConfigureAwait(false);
            //TODO: why are we not removing the history here?
            paymentLogger.LogInfo($"Finished resetting actor for apprenticeship {apprenticeshipKeyString}");
        }
    }
}