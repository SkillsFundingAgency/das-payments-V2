using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Processors
{
    public class RefundRemovedLearningAimProcessor : IRefundRemovedLearningAimProcessor
    {
        private readonly IRefundRemovedLearningAimService refundRemovedLearningAimService;
        private readonly IPaymentLogger logger;
        private readonly IMapper mapper;
        private readonly IPeriodisedRequiredPaymentEventFactory requiredPaymentEventFactory;

        public RefundRemovedLearningAimProcessor(IRefundRemovedLearningAimService refundRemovedLearningAimService, IPaymentLogger logger, IMapper mapper, IPeriodisedRequiredPaymentEventFactory requiredPaymentEventFactory)
        {
            this.refundRemovedLearningAimService = refundRemovedLearningAimService ?? throw new ArgumentNullException(nameof(refundRemovedLearningAimService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.requiredPaymentEventFactory = requiredPaymentEventFactory ?? throw new ArgumentNullException(nameof(requiredPaymentEventFactory));
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> RefundLearningAim(IdentifiedRemovedLearningAim identifiedRemovedLearningAim, IDataCache<PaymentHistoryEntity[]> paymentHistoryCache, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Now processing request to generate refunds for learning aim: learner: {identifiedRemovedLearningAim.Learner.ReferenceNumber}, Aim: {identifiedRemovedLearningAim.LearningAim.Reference}");
            var cacheItem = await paymentHistoryCache.TryGet(CacheKeys.PaymentHistoryKey, cancellationToken)
                .ConfigureAwait(false);
            if (!cacheItem.HasValue)
                throw new InvalidOperationException("No payment history found in the cache.");

            var historicPayments = cacheItem.Value.Select(mapper.Map<PaymentHistoryEntity, Payment>).ToList();
            logger.LogDebug($"Got {historicPayments.Count} historic payments. Now generating refunds per transaction type.");

            // Now restricted the historic payments from the cache to those for the transaction type in that particular group.
            var requiredPaymentEvents = historicPayments.GroupBy(historicPayment => historicPayment.TransactionType)
                .SelectMany(group => CreateRefundPayments(
                    identifiedRemovedLearningAim, 
                    historicPayments.Where(h => h.TransactionType == group.Key).ToList(), 
                    group.Key, 
                    cacheItem.Value.Where(c => c.TransactionType == group.Key).ToList()))
                .ToList();

            return requiredPaymentEvents.AsReadOnly();
        }

        private IList<PeriodisedRequiredPaymentEvent> CreateRefundPayments(IdentifiedRemovedLearningAim identifiedRemovedLearningAim, List<Payment> historicPayments, int transactionType, List<PaymentHistoryEntity> cacheItem)
        {
            var refundPaymentsAndPeriods = refundRemovedLearningAimService.RefundLearningAim(historicPayments);

            return refundPaymentsAndPeriods.Where(r => r.payment.TransactionType == transactionType)
                .Select(refund =>
                {
                    logger.LogVerbose("Now mapping the required payment to a PeriodisedRequiredPaymentEvent.");

                    var historicPayment = cacheItem.FirstOrDefault(payment =>
                        payment.PriceEpisodeIdentifier == refund.payment.PriceEpisodeIdentifier &&
                        payment.DeliveryPeriod == refund.deliveryPeriod
                        && payment.TransactionType == transactionType);

                    if (historicPayment == null)
                        throw new InvalidOperationException($"Cannot find historic payment with price episode identifier: {refund.payment.PriceEpisodeIdentifier} for period {refund.deliveryPeriod}.");
                    
                    var requiredPaymentEvent = requiredPaymentEventFactory.Create(refund.payment.EarningType, transactionType);
                    if (requiredPaymentEvent == null)
                    {
                        // This shouldn't now happen as the transaction type in the history should match the one in the cache
                        logger.LogWarning($"Required payment event is null for EarningType: {refund.payment.EarningType} with TransactionType: {transactionType}");
                        return null;
                    }
                    mapper.Map(refund.payment, requiredPaymentEvent);
                    mapper.Map(historicPayment, requiredPaymentEvent);
                    mapper.Map(identifiedRemovedLearningAim, requiredPaymentEvent);

                    // funding line type is not part of removed aim, we need to use value from historic payment
                    requiredPaymentEvent.LearningAim.FundingLineType = historicPayment.LearningAimFundingLineType;

                    logger.LogDebug("Finished mapping");
                    return requiredPaymentEvent;
                }).Where(x =>
                {
                    // Just in case the null is hit above, remove any nulls
                    return x != null;
                }).ToList();
        }
    }
}