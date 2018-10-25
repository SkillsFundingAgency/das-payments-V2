using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Model;
using SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Interfaces;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService
{

    [StatePersistence(StatePersistence.Volatile)]
    public class ProviderPaymentsService : Actor, IProviderPaymentsService
    {
        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IValidatePaymentMessage validatePaymentMessage;
        private readonly IPaymentLogger paymentLogger;
        private  IProviderPaymentsHandlerService paymentsHandlerService;
        private readonly long ukprn;

        public ProviderPaymentsService(ActorService actorService, ActorId actorId,
            IProviderPaymentsRepository providerPaymentsRepository,
            IValidatePaymentMessage validatePaymentMessage,
            IPaymentLogger paymentLogger)
            : base(actorService, actorId)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
            this.validatePaymentMessage = validatePaymentMessage;
            this.paymentLogger = paymentLogger;

            ukprn = actorId.GetLongId();
        }

        public async Task ProcessPayment(ProviderPeriodicPayment message, CancellationToken cancellationToken)
        {
            await paymentsHandlerService.ProcessPayment(message, cancellationToken);
        }

        public async Task HandleMonthEnd(short collectionYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            var providerPayments = await paymentsHandlerService.GetMonthEndPayments(collectionYear, collectionPeriod, ukprn, cancellationToken);
            //TODO Publish Events
        }

        protected override async Task OnActivateAsync()
        {
            var reliableCollectionCache = new ReliableCollectionCache<IlrSubmittedEvent>(StateManager);
            paymentsHandlerService = new ProviderPaymentsHandlerService(providerPaymentsRepository, reliableCollectionCache, validatePaymentMessage, paymentLogger);

            await base.OnActivateAsync().ConfigureAwait(false);
        }

    }
}
