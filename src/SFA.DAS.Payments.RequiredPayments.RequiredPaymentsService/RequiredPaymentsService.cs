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
        private ApprenticeshipContractType2PaymentDueEventHanlder _act2PaymentDueEventHanlder;
        private ReliableCollectionCache<PaymentEntity[]> _paymentHistoryCache;

        private readonly IPaymentLogger _paymentLogger;
        private readonly string _apprenticeshipKey;
        private readonly IApprenticeshipKeyService _apprenticeshipKeyService;
        private readonly IApprenticeshipContractType2PaymentDueProcessor _act2PaymentDueProcessor;
        private readonly IMapper _mapper;
        private readonly IPaymentHistoryRepository _paymentHistoryRepository;

        public RequiredPaymentsService(ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger,
            IApprenticeshipKeyService apprenticeshipKeyService,
            IApprenticeshipContractType2PaymentDueProcessor act2PaymentDueProcessor, IMapper mapper, IPaymentHistoryRepository paymentHistoryRepository) : base(actorService, actorId)
        {
            _paymentLogger = paymentLogger;
            _apprenticeshipKeyService = apprenticeshipKeyService;
            _act2PaymentDueProcessor = act2PaymentDueProcessor;
            _mapper = mapper;
            _paymentHistoryRepository = paymentHistoryRepository;
            _apprenticeshipKey = actorId.GetStringId();
        }

        public async Task<ApprenticeshipContractType2RequiredPaymentEvent> HandleAct2PaymentDueEvent(ApprenticeshipContractType2PaymentDueEvent paymentDueEvent, CancellationToken cancellationToken)
        {
            _paymentLogger.LogVerbose($"Handling PaymentDue for {_apprenticeshipKey}");

            if (!await IsInitialised().ConfigureAwait(false))
                await Initialise().ConfigureAwait(false);

            var requiredPaymentEvents = await _act2PaymentDueEventHanlder.HandlePaymentDue(paymentDueEvent, cancellationToken).ConfigureAwait(false);

            return requiredPaymentEvents;
        }

        protected override async Task OnActivateAsync()
        {
            _paymentHistoryCache = new ReliableCollectionCache<PaymentEntity[]>(StateManager);

            _act2PaymentDueEventHanlder = new ApprenticeshipContractType2PaymentDueEventHanlder(
                _act2PaymentDueProcessor,
                _paymentHistoryCache,
                _mapper,
                _apprenticeshipKeyService,
                _paymentHistoryRepository,
                _apprenticeshipKey
            );

            if (!await IsInitialised().ConfigureAwait(false))
                await Initialise().ConfigureAwait(false);

            await base.OnActivateAsync().ConfigureAwait(false);
        }

        public async Task Initialise()
        {
            _paymentLogger.LogInfo($"Initialising actor for apprenticeship {_apprenticeshipKey}");

            //await _act2PaymentDueEventHanlder.PopulatePaymentHistoryCache(CancellationToken.None).ConfigureAwait(false);

            _paymentLogger.LogInfo($"Initialised actor for apprenticeship {_apprenticeshipKey}");

            await StateManager.AddStateAsync("initialised", true).ConfigureAwait(false);
        }

        // TODO: update payment history when new payments created

        private async Task<bool> IsInitialised()
        {
            return await StateManager.ContainsStateAsync("initialised").ConfigureAwait(false);
        }
    }
}