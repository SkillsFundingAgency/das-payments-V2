﻿using System;
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
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure;
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
        private readonly IMapper mapper;
        private readonly IHoldingBackCompletionPaymentService completionPaymentService;
        private readonly IPaymentHistoryRepository paymentHistoryRepository;
        private readonly IApprenticeshipKeyProvider apprenticeshipKeyProvider;
        private readonly INegativeEarningService negativeEarningService;

        protected EarningEventProcessorBase(
            IMapper mapper, 
            IRequiredPaymentProcessor requiredPaymentProcessor, 
            IHoldingBackCompletionPaymentService completionPaymentService, 
            IPaymentHistoryRepository paymentHistoryRepository, 
            IApprenticeshipKeyProvider apprenticeshipKeyProvider, 
            INegativeEarningService negativeEarningService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.requiredPaymentProcessor = requiredPaymentProcessor ?? throw new ArgumentNullException(nameof(requiredPaymentProcessor));
            this.completionPaymentService = completionPaymentService;
            this.paymentHistoryRepository = paymentHistoryRepository;
            this.apprenticeshipKeyProvider = apprenticeshipKeyProvider;
            this.negativeEarningService = negativeEarningService;
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> HandleEarningEvent(TEarningEvent earningEvent, IDataCache<PaymentHistoryEntity[]> paymentHistoryCache, CancellationToken cancellationToken)
        {
            if (earningEvent == null)
                throw new ArgumentNullException(nameof(earningEvent));

            var result = new List<PeriodisedRequiredPaymentEvent>();
            
            var cachedPayments = await paymentHistoryCache.TryGet(CacheKeys.PaymentHistoryKey, cancellationToken);
            var academicYearPayments = cachedPayments.HasValue
                ? cachedPayments.Value
                    .Where(p => p.LearnAimReference == earningEvent.LearningAim.Reference)
                    .Select(p => mapper.Map<PaymentHistoryEntity, Payment>(p))
                    .ToList()
                : new List<Payment>();

            foreach (var (period, type) in GetPeriods(earningEvent))
            {
                if (period.Period > earningEvent.CollectionPeriod.Period) // cut off future periods
                    continue;

                if (period.Amount != 0 && !period.SfaContributionPercentage.HasValue)
                {
                    throw new InvalidOperationException("Non-zero amount with no Sfa Contribution");
                }

                var payments = academicYearPayments.Where(payment => payment.DeliveryPeriod == period.Period &&
                                                                     payment.TransactionType == type)
                    .ToList();

                List<RequiredPayment> requiredPayments;
                var holdBackCompletionPayments = false;

                if (period.Amount < 0 && 
                    NegativeEarningWillResultInARefund(period, payments))
                {
                    requiredPayments = negativeEarningService
                        .ProcessNegativeEarning(period.Amount, academicYearPayments, period.Period, period.PriceEpisodeIdentifier);
                }
                else
                {
                    var earning = new Earning
                    {
                        Amount = period.Amount,
                        SfaContributionPercentage = period.SfaContributionPercentage,
                        EarningType = GetEarningType(type),
                        PriceEpisodeIdentifier = period.PriceEpisodeIdentifier,
                    };

                    requiredPayments = requiredPaymentProcessor.GetRequiredPayments(earning, payments);
                    if (requiredPayments.Count > 0)
                    {
                        holdBackCompletionPayments = await HoldBackCompletionPayments(earningEvent, earning, type, cancellationToken).ConfigureAwait(false);
                    }
                }

                if (requiredPayments.Sum(x => x.Amount) == 0)
                {
                    continue;
                }

                foreach (var requiredPayment in requiredPayments)
                {
                    var requiredPaymentEvent = CreateRequiredPaymentEvent(requiredPayment.EarningType, type, holdBackCompletionPayments);
                    mapper.Map(period, requiredPaymentEvent);
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

                    result.Add(requiredPaymentEvent);
                }
            }

            return result.AsReadOnly();
        }

        private static bool NegativeEarningWillResultInARefund(EarningPeriod period, List<Payment> payments)
        {
            return period.Amount < payments.Sum(x => x.Amount);
        }

        private async Task<bool> HoldBackCompletionPayments(TEarningEvent earningEvent, Earning earning, int type, CancellationToken cancellationToken)
        {
            if (type != (int) OnProgrammeEarningType.Completion)
                return false;

            var priceEpisode = earningEvent.PriceEpisodes.Single(p => p.Identifier == earning.PriceEpisodeIdentifier);
            var key = apprenticeshipKeyProvider.GetCurrentKey();
            var employerPayments = await paymentHistoryRepository.GetEmployerCoInvestedPaymentHistoryTotal(key, cancellationToken).ConfigureAwait(false);

            return completionPaymentService.ShouldHoldBackCompletionPayment(employerPayments, priceEpisode);
        }

        protected abstract EarningType GetEarningType(int type);

        protected PeriodisedRequiredPaymentEvent CreateRequiredPaymentEvent(EarningType earningType, int transactionType, bool holdBackCompletionPayment)
        {
            if (holdBackCompletionPayment)
                return new CompletionPaymentHeldBackEvent();

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