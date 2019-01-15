using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Application;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService
{
    [StatePersistence(StatePersistence.Volatile)]
    public class RequiredPaymentsService : Actor, IRequiredPaymentsService
    {
        private ReliableCollectionCache<PaymentHistoryEntity[]> paymentHistoryCache;

        private readonly IPaymentLogger paymentLogger;
        private readonly ApprenticeshipKey apprenticeshipKey;
        private readonly IPaymentHistoryRepository paymentHistoryRepository;
        private readonly IPaymentKeyService paymentKeyService;
        private readonly ILifetimeScope lifetimeScope;
        private const string InitialisedKey = "initialised";

        public RequiredPaymentsService(ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger,
            IApprenticeshipKeyService apprenticeshipKeyService,
            IPaymentHistoryRepository paymentHistoryRepository,
            IPaymentKeyService paymentKeyService,
            ILifetimeScope lifetimeScope) : base(actorService, actorId)
        {
            this.paymentLogger = paymentLogger;
            this.paymentHistoryRepository = paymentHistoryRepository;
            this.paymentKeyService = paymentKeyService ?? throw new ArgumentNullException(nameof(paymentKeyService));
            this.lifetimeScope = lifetimeScope;
            apprenticeshipKey = apprenticeshipKeyService.ParseApprenticeshipKey(actorId.GetStringId());
        }

        public async Task<ReadOnlyCollection<RequiredPaymentEvent>> HandleEarningEvent(EarningEvent earningEvent, CancellationToken cancellationToken)
        {
            paymentLogger.LogVerbose($"Handling EarningEvent for {apprenticeshipKey}");

            await Initialise().ConfigureAwait(false);

            var handler = lifetimeScope.ResolveKeyed<IEarningEventHandler>(earningEvent.GetType());
            
            var requiredPaymentEvents = await handler.HandleEarningEvent(earningEvent, paymentHistoryCache, cancellationToken).ConfigureAwait(false);

            return requiredPaymentEvents;
        }

        protected override async Task OnActivateAsync()
        {   
            paymentHistoryCache = new ReliableCollectionCache<PaymentHistoryEntity[]>(StateManager);

            await Initialise().ConfigureAwait(false);

            await base.OnActivateAsync().ConfigureAwait(false);
        }

        public async Task Initialise()
        {
            if (await StateManager.ContainsStateAsync(InitialisedKey).ConfigureAwait(false)) return;
            
            paymentLogger.LogInfo($"Initialising actor for apprenticeship {apprenticeshipKey}");

            var paymentHistory = await paymentHistoryRepository.GetPaymentHistory(apprenticeshipKey, CancellationToken.None).ConfigureAwait(false);

            var groupedHistory = paymentHistory.GroupBy(payment => paymentKeyService.GeneratePaymentKey(payment.LearnAimReference, payment.TransactionType, payment.CollectionPeriod.AcademicYear, payment.DeliveryPeriod))
                .ToDictionary(c => c.Key, c => c.ToArray());

            foreach (var group in groupedHistory)
            {
                await paymentHistoryCache.AddOrReplace(group.Key, group.Value, CancellationToken.None).ConfigureAwait(false);
            }

            paymentLogger.LogInfo($"Initialised actor for apprenticeship {apprenticeshipKey}");

            await StateManager.TryAddStateAsync(InitialisedKey, true).ConfigureAwait(false);
        }

        public async Task Reset()
        {
            paymentLogger.LogInfo($"Resetting actor for apprenticeship {apprenticeshipKey}");
            await StateManager.TryRemoveStateAsync(InitialisedKey, CancellationToken.None).ConfigureAwait(false);
        }
    }
}