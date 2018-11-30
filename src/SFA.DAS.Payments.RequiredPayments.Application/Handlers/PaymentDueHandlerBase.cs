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
    public abstract class PaymentDueHandlerBase<TPaymentDue, TRequiredPayment>
        where TPaymentDue : PaymentDueEvent
        where TRequiredPayment : RequiredPaymentEvent
    {
        private readonly IPaymentKeyService paymentKeyService;
        private readonly IPaymentDueProcessor paymentDueProcessor;
        private readonly IRepositoryCache<PaymentHistoryEntity[]> paymentHistoryCache;
        private readonly IMapper mapper;
        private readonly IPaymentHistoryRepository paymentHistoryRepository;
        private readonly IApprenticeshipKeyService apprenticeshipKeyService;
        private readonly string apprenticeshipKey;

        protected PaymentDueHandlerBase(IPaymentKeyService paymentKeyService, IPaymentDueProcessor paymentDueProcessor, IRepositoryCache<PaymentHistoryEntity[]> paymentHistoryCache, IMapper mapper, IPaymentHistoryRepository paymentHistoryRepository, IApprenticeshipKeyService apprenticeshipKeyService, string apprenticeshipKey)
        {
            this.paymentKeyService = paymentKeyService ?? throw new ArgumentNullException(nameof(paymentKeyService));
            this.paymentDueProcessor = paymentDueProcessor;
            this.paymentHistoryCache = paymentHistoryCache;
            this.mapper = mapper;
            this.paymentHistoryRepository = paymentHistoryRepository;
            this.apprenticeshipKeyService = apprenticeshipKeyService;
            this.apprenticeshipKey = apprenticeshipKey;
        }

        public async Task<TRequiredPayment> HandlePaymentDue(TPaymentDue paymentDue,
            CancellationToken cancellationToken)
        {
            if (paymentDue == null)
                throw new ArgumentNullException(nameof(paymentDue));

            var key = paymentKeyService.GeneratePaymentKey(paymentDue.PriceEpisodeIdentifier,
                paymentDue.LearningAim.Reference, GetTransactionType(paymentDue), paymentDue.DeliveryPeriod);

            var paymentHistoryValue = await paymentHistoryCache.TryGet(key, cancellationToken);

            var payments = paymentHistoryValue.HasValue
                ? paymentHistoryValue.Value.Select(p => mapper.Map<PaymentHistoryEntity, Payment>(p)).ToArray()
                : new Payment[0];

            var amountDue = paymentDueProcessor.CalculateRequiredPaymentAmount(paymentDue.AmountDue, payments);

            if (amountDue == 0)
                return null;

            var requiredPayment = CreateRequiredPayment(paymentDue);

            requiredPayment.AmountDue = amountDue;
            requiredPayment.Learner = paymentDue.Learner.Clone();
            requiredPayment.Ukprn = paymentDue.Ukprn;
            requiredPayment.CollectionPeriod = paymentDue.CollectionPeriod.Clone();
            requiredPayment.DeliveryPeriod = paymentDue.DeliveryPeriod.Clone();
            requiredPayment.LearningAim = paymentDue.LearningAim.Clone();
            requiredPayment.PriceEpisodeIdentifier = paymentDue.PriceEpisodeIdentifier;
            requiredPayment.EventTime = DateTimeOffset.UtcNow;
            requiredPayment.JobId = paymentDue.JobId;
            requiredPayment.IlrSubmissionDateTime = paymentDue.IlrSubmissionDateTime;

            return requiredPayment;
        }

        protected abstract TRequiredPayment CreateRequiredPayment(TPaymentDue paymentDue);

        protected abstract int GetTransactionType(TPaymentDue paymentDue);

        public async Task PopulatePaymentHistoryCache(CancellationToken cancellationToken)
        {
            var paymentHistory = await paymentHistoryRepository.GetPaymentHistory(apprenticeshipKeyService.ParseApprenticeshipKey(apprenticeshipKey), cancellationToken).ConfigureAwait(false);

            if (paymentHistory != null)
            {
                var groupedEntities = paymentHistory
                    .GroupBy(payment => paymentKeyService.GeneratePaymentKey(payment.PriceEpisodeIdentifier, payment.LearnAimReference, payment.TransactionType, new CalendarPeriod(payment.DeliveryPeriod)))
                    .ToDictionary(c => c.Key, c => c.ToArray());

                foreach (var p in groupedEntities)
                {
                    if (await paymentHistoryCache.Contains(p.Key, cancellationToken).ConfigureAwait(false))
                        await paymentHistoryCache.Clear(p.Key, cancellationToken).ConfigureAwait(false);

                    await paymentHistoryCache.Add(p.Key, p.Value, CancellationToken.None).ConfigureAwait(false);
                }
            }
        }

    }
}