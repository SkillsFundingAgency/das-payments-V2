using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public abstract class EarningEventProcessorBase<TEarningEvent> : IEarningEventProcessor<TEarningEvent>
        where TEarningEvent : IEarningEvent
    {
        protected readonly IPaymentDueProcessor paymentDueProcessor;
        private readonly IPaymentKeyService paymentKeyService;
        private readonly IMapper mapper;

        protected EarningEventProcessorBase(IPaymentKeyService paymentKeyService, IMapper mapper, IPaymentDueProcessor paymentDueProcessor)
        {
            this.paymentKeyService = paymentKeyService ?? throw new ArgumentNullException(nameof(paymentKeyService));
            this.mapper = mapper;
            this.paymentDueProcessor = paymentDueProcessor;
        }

        public async Task<ReadOnlyCollection<RequiredPaymentEvent>> HandleEarningEvent(TEarningEvent earningEvent, IDataCache<PaymentHistoryEntity[]> paymentHistoryCache, CancellationToken cancellationToken)
        {
            if (earningEvent == null)
                throw new ArgumentNullException(nameof(earningEvent));

            var result = new List<RequiredPaymentEvent>();

            foreach (var periodAndType in GetPeriods(earningEvent))
            {
                if (periodAndType.period.Period > earningEvent.CollectionPeriod.Period) // cut off future periods
                    continue;

                var key = paymentKeyService.GeneratePaymentKey(earningEvent.LearningAim.Reference, periodAndType.type, earningEvent.CollectionYear, periodAndType.period.Period);
                var paymentHistoryValue = await paymentHistoryCache.TryGet(key, cancellationToken);

                var payments = paymentHistoryValue.HasValue
                    ? paymentHistoryValue.Value.Select(p => mapper.Map<PaymentHistoryEntity, Payment>(p)).ToArray()
                    : new Payment[0];

                var amountDue = paymentDueProcessor.CalculateRequiredPaymentAmount(periodAndType.period.Amount, payments);

                if (amountDue == 0)
                    continue;

                string priceEpisodeIdentifier;

                if (amountDue < 0 && payments.Length > 0) // refunds need to use price episode ID that they are refunding
                    priceEpisodeIdentifier = payments[0].PriceEpisodeIdentifier;
                else
                    priceEpisodeIdentifier = periodAndType.period.PriceEpisodeIdentifier;

                var requiredPayment = CreateRequiredPayment(earningEvent, periodAndType, payments);

                requiredPayment.AmountDue = amountDue;
                requiredPayment.DeliveryPeriod = periodAndType.period.Period;
                requiredPayment.PriceEpisodeIdentifier = priceEpisodeIdentifier;

                mapper.Map(earningEvent, requiredPayment);

                result.Add(requiredPayment);
            }

            return result.AsReadOnly();

        }

        protected abstract RequiredPaymentEvent CreateRequiredPayment(TEarningEvent earningEvent, (EarningPeriod period, int type) periodAndType, Payment[] payments);

        protected abstract IReadOnlyCollection<(EarningPeriod period, int type)> GetPeriods(TEarningEvent earningEvent);
    }
}