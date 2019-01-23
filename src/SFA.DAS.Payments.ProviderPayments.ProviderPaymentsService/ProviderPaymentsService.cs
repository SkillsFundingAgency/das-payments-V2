using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IProviderPaymentsService = SFA.DAS.Payments.ProviderPayments.Application.Services.IProviderPaymentsService;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService
{

    [StatePersistence(StatePersistence.Volatile)]
    public class ProviderPaymentsService : Actor, Interfaces.IProviderPaymentsService
    {
        private readonly ActorId actorId;
        private readonly IProviderPaymentsRepository providerPaymentsRepository;
        private readonly IValidateIlrSubmission validateIlrSubmission;
        private readonly IPaymentLogger paymentLogger;
        private IProviderPaymentsService paymentsService;
        private IHandleIlrSubmissionService handleIlrSubmissionService;
        private IMonthEndService monthEndService;
        private long ukprn;

        public ProviderPaymentsService(ActorService actorService, ActorId actorId,
            IProviderPaymentsRepository providerPaymentsRepository,
            IValidateIlrSubmission validateIlrSubmission,
            IPaymentLogger paymentLogger)
            : base(actorService, actorId)
        {
            this.actorId = actorId;
            this.providerPaymentsRepository = providerPaymentsRepository ?? throw new ArgumentNullException(nameof(providerPaymentsRepository));
            this.validateIlrSubmission = validateIlrSubmission ?? throw new ArgumentNullException(nameof(validateIlrSubmission));
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
        }

        public async Task HandlePayment(PaymentModel message, CancellationToken cancellationToken)
        {
            await paymentsService.ProcessPayment(message, cancellationToken);
        }

        public async Task<List<PaymentModel>> GetMonthEndPayments(string collectionPeriodName,
            CancellationToken cancellationToken)
        {
            return await monthEndService.GetMonthEndPayments(collectionPeriodName, ukprn, cancellationToken);
        }

        public async Task HandleIlrSubMission(IlrSubmittedEvent message, CancellationToken cancellationToken)
        {
            await handleIlrSubmissionService.Handle(message, cancellationToken);
        }

        protected override async Task OnActivateAsync()
        {
            if (!long.TryParse(actorId.GetStringId(), out ukprn)) throw new Exception("Unable to cast Actor Id to Ukprn");

            var reliableCollectionCache = new ReliableCollectionCache<IlrSubmittedEvent>(StateManager);
            paymentsService = new Application.Services.ProviderPaymentsService(providerPaymentsRepository, reliableCollectionCache, validateIlrSubmission, paymentLogger);
            handleIlrSubmissionService = new HandleIlrSubmissionService(providerPaymentsRepository, reliableCollectionCache, validateIlrSubmission, paymentLogger);
            monthEndService = new MonthEndService(providerPaymentsRepository);
            await base.OnActivateAsync().ConfigureAwait(false);
        }
    }
}
