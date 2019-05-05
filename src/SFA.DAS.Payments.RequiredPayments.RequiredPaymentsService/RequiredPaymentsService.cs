using System;
using System.Collections;
using System.Collections.ObjectModel;
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
        private readonly IPaymentHistoryRepository paymentHistoryRepository;
        private readonly IPaymentKeyService paymentKeyService;
        private readonly IApprenticeshipContractType2EarningsEventProcessor contractType2EarningsEventProcessor;
        private readonly IFunctionalSkillEarningsEventProcessor functionalSkillEarningsEventProcessor;
        private readonly IPayableEarningEventProcessor payableEarningEventProcessor;
        readonly ITelemetry telemetry;


        public RequiredPaymentsService(ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger,
            IApprenticeshipKeyService apprenticeshipKeyService,
            IPaymentHistoryRepository paymentHistoryRepository,
            IPaymentKeyService paymentKeyService, 
            IApprenticeshipContractType2EarningsEventProcessor contractType2EarningsEventProcessor, 
            IFunctionalSkillEarningsEventProcessor functionalSkillEarningsEventProcessor, 
            IPayableEarningEventProcessor payableEarningEventProcessor, ITelemetry telemetry) 
            : base(actorService, actorId)
        {
            this.paymentLogger = paymentLogger;
            this.paymentHistoryRepository = paymentHistoryRepository;
            this.paymentKeyService = paymentKeyService ?? throw new ArgumentNullException(nameof(paymentKeyService));
            this.contractType2EarningsEventProcessor = contractType2EarningsEventProcessor;
            this.functionalSkillEarningsEventProcessor = functionalSkillEarningsEventProcessor;
            this.payableEarningEventProcessor = payableEarningEventProcessor;
            this.telemetry = telemetry;
            apprenticeshipKeyString = actorId.GetStringId();
            apprenticeshipKey = apprenticeshipKeyService.ParseApprenticeshipKey(apprenticeshipKeyString);
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleApprenticeship2ContractTypeEarningsEvent(ApprenticeshipContractType2EarningEvent earningEvent, CancellationToken cancellationToken)
        {
            paymentLogger.LogVerbose($"Handling ApprenticeshipContractType2EarningEvent for {apprenticeshipKeyString}");

            using (var operation = telemetry.StartOperation())
            {
                await Initialise().ConfigureAwait(false);
                var requiredPaymentEvents = await contractType2EarningsEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCache, cancellationToken).ConfigureAwait(false);
                Log(requiredPaymentEvents);
                telemetry.StopOperation(operation);
                return requiredPaymentEvents;
            }
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleFunctionalSkillEarningsEvent(FunctionalSkillEarningsEvent earningEvent, CancellationToken cancellationToken)
        {
            paymentLogger.LogVerbose($"Handling FunctionalSkillEarningsEvent for {apprenticeshipKeyString}");

            using (var operation = telemetry.StartOperation())
            {
                await Initialise().ConfigureAwait(false);
                var requiredPaymentEvents = await functionalSkillEarningsEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCache, cancellationToken).ConfigureAwait(false);
                Log(requiredPaymentEvents);
                telemetry.StopOperation(operation);
                return requiredPaymentEvents;
            }
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandlePayableEarningEvent(PayableEarningEvent earningEvent, CancellationToken cancellationToken)
        {
            paymentLogger.LogVerbose($"Handling PayableEarningEvent for {apprenticeshipKeyString}");

            using (var operation = telemetry.StartOperation())
            {
                await Initialise().ConfigureAwait(false);
                var requiredPaymentEvents = await payableEarningEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCache, cancellationToken).ConfigureAwait(false);
                Log(requiredPaymentEvents);
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
            paymentHistoryCache = new ReliableCollectionCache<PaymentHistoryEntity[]>(StateManager);

            await Initialise().ConfigureAwait(false);

            await base.OnActivateAsync().ConfigureAwait(false);
        }

        public async Task Initialise()
        {
            if (await StateManager.ContainsStateAsync(CacheKeys.InitialisedKey).ConfigureAwait(false)) return;
            
            paymentLogger.LogDebug($"Initialising actor for apprenticeship {apprenticeshipKeyString}");

            var paymentHistory = await paymentHistoryRepository.GetPaymentHistory(apprenticeshipKey, CancellationToken.None).ConfigureAwait(false);
            await paymentHistoryCache.AddOrReplace(CacheKeys.PaymentHistoryKey, paymentHistory.ToArray(), CancellationToken.None).ConfigureAwait(false);
            await StateManager.TryAddStateAsync(CacheKeys.InitialisedKey, true).ConfigureAwait(false);
            paymentLogger.LogInfo($"Initialised actor for apprenticeship {apprenticeshipKeyString}");
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