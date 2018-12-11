using AutoMapper;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.Handlers
{
    public class ApprenticeshipContractType2PaymentDueEventHandler : PaymentDueHandlerBase<ApprenticeshipContractType2PaymentDueEvent, ApprenticeshipContractType2RequiredPaymentEvent>
    {
        public ApprenticeshipContractType2PaymentDueEventHandler(IPaymentDueProcessor paymentDueProcessor, IMapper mapper, IPaymentKeyService paymentKeyService)
            : base(paymentKeyService, paymentDueProcessor, mapper)
        {
        }

        protected override ApprenticeshipContractType2RequiredPaymentEvent CreateRequiredPayment(ApprenticeshipContractType2PaymentDueEvent paymentDue)
        {
            if (paymentDue == null)
                throw new ArgumentNullException(nameof(paymentDue));

            var key = paymentKeyService.GeneratePaymentKey(paymentDue.LearningAim.Reference, (int)paymentDue.Type, paymentDue.DeliveryPeriod);

            var paymentHistoryValue = await paymentHistoryCache.TryGet(key, cancellationToken);

            var payments = paymentHistoryValue.HasValue ? paymentHistoryValue.Value.Select(p => mapper.Map<PaymentHistoryEntity, Payment>(p)).ToArray() : new Payment[0];

            var amountDue = act2PaymentDueProcessor.CalculateRequiredPaymentAmount(paymentDue.AmountDue, payments);

            if (amountDue == 0)
                return null;

            string priceEpisodeIdentifier;

            if (amountDue < 0 && payments.Length > 0)
                priceEpisodeIdentifier = payments[0].PriceEpisodeIdentifier;
            else
                priceEpisodeIdentifier = paymentDue.PriceEpisodeIdentifier;

            return new ApprenticeshipContractType2RequiredPaymentEvent
            {
                AmountDue = amountDue,
                Learner = paymentDue.Learner.Clone(),
                Ukprn = paymentDue.Ukprn,
                CollectionPeriod = paymentDue.CollectionPeriod.Clone(),
                DeliveryPeriod = paymentDue.DeliveryPeriod.Clone(),
                LearningAim = paymentDue.LearningAim.Clone(),
                PriceEpisodeIdentifier = priceEpisodeIdentifier,
                OnProgrammeEarningType = paymentDue.Type,
                SfaContributionPercentage = paymentDue.SfaContributionPercentage,
            };
        }

        protected override int GetTransactionType(ApprenticeshipContractType2PaymentDueEvent paymentDue)
        {
            var paymentHistory = await paymentHistoryRepository.GetPaymentHistory(apprenticeshipKeyService.ParseApprenticeshipKey(apprenticeshipKey), cancellationToken).ConfigureAwait(false);

            if (paymentHistory != null)
            {
                var groupedEntities = paymentHistory
                    .GroupBy(payment => paymentKeyService.GeneratePaymentKey(payment.LearnAimReference, payment.TransactionType, new CalendarPeriod(payment.DeliveryPeriod)))
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