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
        private IIlrSubmissionService ilrSubmissionService;
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

        public async Task ProcessPayment(PaymentModel message, CancellationToken cancellationToken)
        {
            await paymentsService.ProcessPayment(message, cancellationToken);
        }

        public async Task<List<PaymentModel>> GetMonthEndPayments(short collectionYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            return await monthEndService.GetMonthEndPayments(collectionYear, collectionPeriod, ukprn, cancellationToken);
        }

        public async Task HandleIlrSubMissionAsync(IlrSubmittedEvent message, CancellationToken cancellationToken)
        {
            await ilrSubmissionService.HandleIlrSubMission(message, cancellationToken);
        }

        protected override async Task OnActivateAsync()
        {
            if (!long.TryParse(actorId.GetStringId(), out ukprn)) throw new Exception("Unable to cast Actor Id to Ukprn");

            var reliableCollectionCache = new ReliableCollectionCache<IlrSubmittedEvent>(StateManager);
            paymentsService = new Application.Services.ProviderPaymentsService(providerPaymentsRepository, reliableCollectionCache, validateIlrSubmission, paymentLogger);
            ilrSubmissionService = new IlrSubmissionService(providerPaymentsRepository, reliableCollectionCache, validateIlrSubmission, paymentLogger);
            monthEndService = new MonthEndService(providerPaymentsRepository, reliableCollectionCache);
            await base.OnActivateAsync().ConfigureAwait(false);
        }
    }
}
