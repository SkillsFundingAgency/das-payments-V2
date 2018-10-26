using System;
using System.Collections.Generic;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Interfaces;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService
{

    [StatePersistence(StatePersistence.Volatile)]
    public class ProviderPaymentsService : Actor, IProviderPaymentsService
    {
        private readonly ActorId actorId;
        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IValidatePaymentMessage validatePaymentMessage;
        private readonly IPaymentLogger paymentLogger;
        private readonly IMapper mapper;
        private readonly IProviderPaymentFactory paymentFactory;
        private  IProviderPaymentsHandlerService paymentsHandlerService;
        private long ukprn;

        public ProviderPaymentsService(ActorService actorService, ActorId actorId,
            IProviderPaymentsRepository providerPaymentsRepository,
            IValidatePaymentMessage validatePaymentMessage,
            IPaymentLogger paymentLogger, IMapper mapper, IProviderPaymentFactory paymentFactory )
            : base(actorService, actorId)
        {
            this.actorId = actorId;
            this.providerPaymentsRepository = providerPaymentsRepository ?? throw new ArgumentNullException(nameof(providerPaymentsRepository));
            this.validatePaymentMessage = validatePaymentMessage ?? throw new ArgumentNullException(nameof(validatePaymentMessage));
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.paymentFactory = paymentFactory ?? throw new ArgumentNullException(nameof(paymentFactory));
        }

        public async Task ProcessPayment(PaymentModel message, CancellationToken cancellationToken)
        {
            await paymentsHandlerService.ProcessPayment(message, cancellationToken);
        }

        public async Task<List<PaymentModel>> GetMonthEndPayments(short collectionYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            return await paymentsHandlerService.GetMonthEndPayments(collectionYear, collectionPeriod, ukprn, cancellationToken);
        }

        protected override async Task OnActivateAsync()
        {
            if (!long.TryParse(actorId.GetStringId(), out ukprn)) throw new Exception($"Unable to cast Actor Id to Ukprn");

            var reliableCollectionCache = new ReliableCollectionCache<IlrSubmittedEvent>(StateManager);
            paymentsHandlerService = new ProviderPaymentsHandlerService(providerPaymentsRepository, reliableCollectionCache, 
                validatePaymentMessage, paymentLogger, mapper,paymentFactory);

            await base.OnActivateAsync().ConfigureAwait(false);
        }
    }
}
