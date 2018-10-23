using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Model;
using SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Interfaces;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService
{

    [StatePersistence(StatePersistence.Volatile)]
    public class ProviderPaymentsService : Actor, IProviderPaymentsService
    {
        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IValidatePaymentMessage validatePaymentMessage;
        private readonly IPaymentLogger paymentLogger;
        private IFundingSourceEventHandlerService fundingSourceEventHandlerService;

        public ProviderPaymentsService(ActorService actorService, ActorId actorId,
            IProviderPaymentsRepository providerPaymentsRepository,
            IValidatePaymentMessage validatePaymentMessage,
            IPaymentLogger paymentLogger)
            : base(actorService, actorId)
        {
            this.providerPaymentsRepository = providerPaymentsRepository;
            this.validatePaymentMessage = validatePaymentMessage;
            this.paymentLogger = paymentLogger;
        }

        public async Task HandleEvent(ProviderPeriodicPayment message, CancellationToken cancellationToken)
        {
            await fundingSourceEventHandlerService.ProcessEvent(message, cancellationToken);
        }

        protected override async Task OnActivateAsync()
        {
            var reliableCollectionCache = new ReliableCollectionCache<IlrSubmittedEvent>(StateManager);
            fundingSourceEventHandlerService = new FundingSourceEventHandlerService(providerPaymentsRepository, reliableCollectionCache, validatePaymentMessage, paymentLogger);

            await base.OnActivateAsync().ConfigureAwait(false);
        }

    }
}
