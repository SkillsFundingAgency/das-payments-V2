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
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;
using Earning = SFA.DAS.Payments.RequiredPayments.Domain.Entities.Earning;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public abstract class EarningEventProcessorBase<TEarningEvent> : IEarningEventProcessor<TEarningEvent>
        where TEarningEvent : IEarningEvent
    {
        private readonly IRequiredPaymentProcessor requiredPaymentProcessor;
        private readonly IPaymentKeyService paymentKeyService;
        private readonly IMapper mapper;

        protected EarningEventProcessorBase(IPaymentKeyService paymentKeyService, IMapper mapper, IRequiredPaymentProcessor requiredPaymentProcessor)
        {
            this.paymentKeyService = paymentKeyService ?? throw new ArgumentNullException(nameof(paymentKeyService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.requiredPaymentProcessor = requiredPaymentProcessor ?? throw new ArgumentNullException(nameof(requiredPaymentProcessor));
        }

        public async Task<ReadOnlyCollection<RequiredPaymentEvent>> HandleEarningEvent(TEarningEvent earningEvent, IDataCache<PaymentHistoryEntity[]> paymentHistoryCache, CancellationToken cancellationToken)
        {
            if (earningEvent == null)
                throw new ArgumentNullException(nameof(earningEvent));

            var result = new List<RequiredPaymentEvent>();

            foreach (var (period, type) in GetPeriods(earningEvent))
            {
                if (period.Period > earningEvent.CollectionPeriod.Period) // cut off future periods
                    continue;

                var key = paymentKeyService.GeneratePaymentKey(earningEvent.LearningAim.Reference, type, earningEvent.CollectionYear, period.Period);
                var paymentHistoryValue = await paymentHistoryCache.TryGet(key, cancellationToken);

                var payments = paymentHistoryValue.HasValue
                    ? paymentHistoryValue.Value.Select(p => mapper.Map<PaymentHistoryEntity, Payment>(p)).ToList()
                    : new List<Payment>();

                if (period.Amount != 0 && !period.SfaContributionPercentage.HasValue)
                {
                    throw new InvalidOperationException("Non-zero amount with no Sfa Contribution");
                }

                var earning = new Earning
                {
                    Amount = period.Amount,
                    SfaContributionPercentage = period.SfaContributionPercentage,
                    EarningType = GetEarningType(type),
                    PriceEpisodeIdentifier = period.PriceEpisodeIdentifier,
                };
                var requiredPayments = requiredPaymentProcessor.GetRequiredPayments(earning, payments);

                if (requiredPayments.Sum(x => x.Amount) == 0)
                {
                    continue;
                }

                foreach (var requiredPayment in requiredPayments)
                {
                    var requiredPaymentEvent = CreateRequiredPaymentEvent(requiredPayment.EarningType, type);

                    mapper.Map(earningEvent, requiredPaymentEvent);
                    mapper.Map(requiredPayment, requiredPaymentEvent);

                    var priceEpisodeIdentifier = requiredPaymentEvent.PriceEpisodeIdentifier;

                    if (earningEvent.PriceEpisodes != null && earningEvent.PriceEpisodes.Any())
                    {
                        var priceEpisode = earningEvent.PriceEpisodes.Count == 1
                            ? earningEvent.PriceEpisodes.FirstOrDefault()
                            : earningEvent.PriceEpisodes?.SingleOrDefault(x => x.Identifier == priceEpisodeIdentifier);

                        mapper.Map(priceEpisode, requiredPaymentEvent);
                    }

                    requiredPaymentEvent.DeliveryPeriod = period.Period;
                    
                    result.Add(requiredPaymentEvent);
                }
            }

            return result.AsReadOnly();
        }

        protected abstract EarningType GetEarningType(int type);

        protected RequiredPaymentEvent CreateRequiredPaymentEvent(EarningType earningType, int transactionType)
        {
            switch (earningType)
            {
                case EarningType.CoInvested:
                    return new CalculatedRequiredCoInvestedAmount
                    {
                        OnProgrammeEarningType = (OnProgrammeEarningType) transactionType,
                    };
                case EarningType.Incentive:
                    return new CalculatedRequiredIncentiveAmount
                    {
                        Type = (IncentivePaymentType) transactionType,
                    };
                case EarningType.Levy:
                    return new CalculatedRequiredLevyAmount
                    {
                        OnProgrammeEarningType = (OnProgrammeEarningType) transactionType,
                    };
            }

            throw new NotImplementedException($"Could not create required payment for earning type: {earningType}");
        }
        
        protected abstract IReadOnlyCollection<(EarningPeriod period, int type)> GetPeriods(TEarningEvent earningEvent);
    }
}