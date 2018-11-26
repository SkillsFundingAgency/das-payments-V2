﻿using System;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Application;
using SFA.DAS.Payments.RequiredPayments.Application.Handlers;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService
{
    [StatePersistence(StatePersistence.Volatile)]
    public class RequiredPaymentsService : Actor, IRequiredPaymentsService
    {
        private ApprenticeshipContractType2PaymentDueEventHandler act2PaymentDueEventHandler;
        private ReliableCollectionCache<PaymentHistoryEntity[]> paymentHistoryCache;

        private readonly IPaymentLogger paymentLogger;
        private readonly string apprenticeshipKey;
        private readonly IApprenticeshipKeyService apprenticeshipKeyService;
        private readonly IApprenticeshipContractType2PaymentDueProcessor act2PaymentDueProcessor;
        private readonly IMapper mapper;
        private readonly IPaymentHistoryRepository paymentHistoryRepository;
        private readonly IPaymentKeyService paymentKeyService;
        private const string InitialisedKey = "initialised";

        public RequiredPaymentsService(ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger,
            IApprenticeshipKeyService apprenticeshipKeyService,
            IApprenticeshipContractType2PaymentDueProcessor act2PaymentDueProcessor, 
            IMapper mapper, 
            IPaymentHistoryRepository paymentHistoryRepository,
            IPaymentKeyService paymentKeyService) : base(actorService, actorId)
        {
            this.paymentLogger = paymentLogger;
            this.apprenticeshipKeyService = apprenticeshipKeyService;
            this.act2PaymentDueProcessor = act2PaymentDueProcessor;
            this.mapper = mapper;
            this.paymentHistoryRepository = paymentHistoryRepository;
            this.paymentKeyService = paymentKeyService ?? throw new ArgumentNullException(nameof(paymentKeyService));
            apprenticeshipKey = actorId.GetStringId();
        }

        public async Task<ApprenticeshipContractType2RequiredPaymentEvent> HandleAct2PaymentDueEvent(ApprenticeshipContractType2PaymentDueEvent paymentDueEvent, CancellationToken cancellationToken)
        {
            paymentLogger.LogVerbose($"Handling PaymentDue for {apprenticeshipKey}");

            await Initialise().ConfigureAwait(false);

            var requiredPaymentEvents = await act2PaymentDueEventHandler.HandlePaymentDue(paymentDueEvent, cancellationToken).ConfigureAwait(false);

            return requiredPaymentEvents;
        }

        protected override async Task OnActivateAsync()
        {
            paymentHistoryCache = new ReliableCollectionCache<PaymentHistoryEntity[]>(StateManager);

            act2PaymentDueEventHandler = new ApprenticeshipContractType2PaymentDueEventHandler(
                act2PaymentDueProcessor,
                paymentHistoryCache,
                mapper,
                apprenticeshipKeyService,
                paymentHistoryRepository,
                apprenticeshipKey, 
                paymentKeyService
            );

            await Initialise().ConfigureAwait(false);

            await base.OnActivateAsync().ConfigureAwait(false);
        }

        public async Task Initialise()
        {
            if (await StateManager.ContainsStateAsync(InitialisedKey).ConfigureAwait(false)) return;
            
            paymentLogger.LogInfo($"Initialising actor for apprenticeship {apprenticeshipKey}");

            await act2PaymentDueEventHandler.PopulatePaymentHistoryCache(CancellationToken.None).ConfigureAwait(false);

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