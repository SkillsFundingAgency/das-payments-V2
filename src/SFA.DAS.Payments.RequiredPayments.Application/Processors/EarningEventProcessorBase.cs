using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Application.Repositories;
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
        protected readonly IRequiredPaymentService requiredPaymentService;
        private readonly IPaymentKeyService paymentKeyService;
        private readonly IMapper mapper;

        protected EarningEventProcessorBase(IPaymentKeyService paymentKeyService, IMapper mapper, IRequiredPaymentService requiredPaymentService)
        {
            this.paymentKeyService = paymentKeyService ?? throw new ArgumentNullException(nameof(paymentKeyService));
            this.mapper = mapper;
            this.requiredPaymentService = requiredPaymentService;
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
                    ? paymentHistoryValue.Value.Select(p => mapper.Map<PaymentHistoryEntity, Payment>(p)).ToList()
                    : new List<Payment>();

                if (periodAndType.period.Amount != 0 && !periodAndType.period.SfaContributionPercentage.HasValue)
                {
                    throw new ArgumentException();
                }

                var earning = new Earning
                {
                    Amount = periodAndType.period.Amount,
                    SfaContributionPercentage = periodAndType.period.SfaContributionPercentage,
                    EarningType = GetEarningType(periodAndType.type),
                };
                var requiredPayments = requiredPaymentService.GetRequiredPayments(earning, payments);

                if (requiredPayments.Sum(x => x.Amount) == 0)
                {
                    continue;
                }

                foreach (var requiredPayment in requiredPayments)
                {
                    var requiredPaymentEvent = CreateRequiredPaymentEvent(requiredPayment.EarningType);

                    string priceEpisodeIdentifier;

                    if (requiredPayment.Amount < 0 && payments.Count > 0) // refunds need to use price episode ID that they are refunding
                        priceEpisodeIdentifier = payments[0].PriceEpisodeIdentifier;
                    else
                        priceEpisodeIdentifier = periodAndType.period.PriceEpisodeIdentifier;

                    requiredPaymentEvent.AmountDue = requiredPayment.Amount;
                    
                    requiredPaymentEvent.DeliveryPeriod = periodAndType.period.Period;
                    requiredPaymentEvent.PriceEpisodeIdentifier = priceEpisodeIdentifier;

                    mapper.Map(earningEvent, requiredPaymentEvent);

                    result.Add(requiredPaymentEvent);
                }
            }

            return result.AsReadOnly();
        }

        protected abstract EarningType GetEarningType(int type);

        protected RequiredPaymentEvent CreateRequiredPaymentEvent(EarningType earningType)
        {
            switch (earningType)
            {
                case EarningType.CoInvested:
                    return new CalculatedRequiredCoInvestedAmount();
                case EarningType.Incentive: 
                    return new CalculatedRequiredIncentiveAmount();
                case EarningType.Levy:
                    return new CalculatedRequiredLevyAmount();
            }

            throw new NotImplementedException($"Could not create required payment for earning type: {earningType}");
        }
        
        protected abstract IReadOnlyCollection<(EarningPeriod period, int type)> GetPeriods(TEarningEvent earningEvent);
    }
}