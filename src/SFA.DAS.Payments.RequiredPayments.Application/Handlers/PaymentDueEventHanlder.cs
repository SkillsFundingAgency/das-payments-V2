using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Handlers
{
    public class Act2PaymentDueEventHanlder : IAct2PaymentDueEventHanlder
    {
        private readonly IAct2PaymentDueProcessor _act2PaymentDueProcessor;
        private readonly IRepositoryCache<PaymentEntity[]> _paymentHistoryCache;
        private readonly IMapper _mapper;
        private readonly IApprenticeshipKeyService _apprenticeshipKeyService;
        private readonly IPaymentHistoryRepository _paymentHistoryRepository;
        private readonly string _apprenticeshipKey;

        public Act2PaymentDueEventHanlder(IAct2PaymentDueProcessor act2PaymentDueProcessor, IRepositoryCache<PaymentEntity[]> paymentHistoryCache, IMapper mapper, IApprenticeshipKeyService apprenticeshipKeyService, IPaymentHistoryRepository paymentHistoryRepository, string apprenticeshipKey)
        {
            _act2PaymentDueProcessor = act2PaymentDueProcessor;
            _paymentHistoryCache = paymentHistoryCache;
            _mapper = mapper;
            _apprenticeshipKeyService = apprenticeshipKeyService;
            _paymentHistoryRepository = paymentHistoryRepository;
            _apprenticeshipKey = apprenticeshipKey;
        }

        public async Task<ApprenticeshipContractType2RequiredPaymentEvent> HandlePaymentDue(ApprenticeshipContractType2PaymentDueEvent paymentDue, CancellationToken cancellationToken)
        {
            if (paymentDue == null)
                throw new ArgumentNullException(nameof(paymentDue));

            var key = _apprenticeshipKeyService.GeneratePaymentKey(paymentDue.PriceEpisodeIdentifier, paymentDue.LearningAim.Reference, paymentDue.TransactionType, paymentDue.DeliveryPeriod);

            var paymentHistoryValue = await _paymentHistoryCache.TryGet(key, cancellationToken);

            var payments = paymentHistoryValue.HasValue ? paymentHistoryValue.Value.Select(p => _mapper.Map<PaymentEntity, Payment>(p)).ToArray() : new Payment[0];

            return _act2PaymentDueProcessor.ProcessPaymentDue(paymentDue, payments);
        }

        public async Task PopulatePaymentHistoryCache(CancellationToken cancellationToken)
        {
            var paymentHistory = await _paymentHistoryRepository.GetPaymentHistory(_apprenticeshipKey, cancellationToken).ConfigureAwait(false);

            if (paymentHistory != null)
            {
                var groupedEntities = paymentHistory
                    .GroupBy(payment => _apprenticeshipKeyService.GeneratePaymentKey(payment.PriceEpisodeIdentifier, payment.LearnAimReference, payment.TransactionType, new CalendarPeriod(payment.DeliveryPeriod)))
                    .ToDictionary(c => c.Key, c => c.ToArray());

                foreach (var p in groupedEntities)
                {
                    await _paymentHistoryCache.Add(p.Key, p.Value, CancellationToken.None).ConfigureAwait(false);
                }
            }

        }
    }
}
