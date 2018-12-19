using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Handlers
{
    public abstract class PaymentDueHandlerBase<TPaymentDue, TRequiredPayment> : IPaymentDueEventHandler
        where TPaymentDue : PaymentDueEvent
        where TRequiredPayment : RequiredPaymentEvent
    {
        private readonly IPaymentKeyService paymentKeyService;
        private readonly IPaymentDueProcessor paymentDueProcessor;
        private readonly IMapper mapper;

        protected PaymentDueHandlerBase(IPaymentKeyService paymentKeyService, IPaymentDueProcessor paymentDueProcessor, IMapper mapper)
        {
            this.paymentKeyService = paymentKeyService ?? throw new ArgumentNullException(nameof(paymentKeyService));
            this.paymentDueProcessor = paymentDueProcessor;
            this.mapper = mapper;
        }

        public async Task<RequiredPaymentEvent> HandlePaymentDue(PaymentDueEvent paymentDue, IRepositoryCache<PaymentHistoryEntity[]> paymentHistoryCache, CancellationToken cancellationToken)
        {
            if (paymentDue == null)
                throw new ArgumentNullException(nameof(paymentDue));

            var key = paymentKeyService.GeneratePaymentKey(paymentDue.LearningAim.Reference, GetTransactionType((TPaymentDue) paymentDue), paymentDue.DeliveryPeriod);

            var paymentHistoryValue = await paymentHistoryCache.TryGet(key, cancellationToken);

            var payments = paymentHistoryValue.HasValue
                ? paymentHistoryValue.Value.Select(p => mapper.Map<PaymentHistoryEntity, Payment>(p)).ToArray()
                : new Payment[0];

            var amountDue = paymentDueProcessor.CalculateRequiredPaymentAmount(paymentDue.AmountDue, payments);

            if (amountDue == 0)
                return null;

            string priceEpisodeIdentifier;

            if (amountDue < 0 && payments.Length > 0)
                priceEpisodeIdentifier = payments[0].PriceEpisodeIdentifier;
            else
                priceEpisodeIdentifier = paymentDue.PriceEpisodeIdentifier;

            var requiredPayment = CreateRequiredPayment((TPaymentDue) paymentDue);

            requiredPayment.AmountDue = amountDue;
            requiredPayment.Learner = paymentDue.Learner.Clone();
            requiredPayment.Ukprn = paymentDue.Ukprn;
            requiredPayment.CollectionPeriod = paymentDue.CollectionPeriod.Clone();
            requiredPayment.DeliveryPeriod = paymentDue.DeliveryPeriod.Clone();
            requiredPayment.LearningAim = paymentDue.LearningAim.Clone();
            requiredPayment.PriceEpisodeIdentifier = priceEpisodeIdentifier;
            requiredPayment.EventTime = DateTimeOffset.UtcNow;
            requiredPayment.JobId = paymentDue.JobId;
            requiredPayment.IlrSubmissionDateTime = paymentDue.IlrSubmissionDateTime;
            

            return requiredPayment;
        }

        protected abstract TRequiredPayment CreateRequiredPayment(TPaymentDue paymentDue);

        protected abstract int GetTransactionType(TPaymentDue paymentDue);
    }
}