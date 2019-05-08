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

            var requiredPaymentEvents = historicPayments.GroupBy(historicPayment => historicPayment.TransactionType)
                .SelectMany(group =>
                    refundRemovedLearningAimService.RefundLearningAim(historicPayments)
                        .Select(requiredPayment =>
                        {
                            logger.LogVerbose($"Now mapping the required payment to a PeriodisedRequiredPaymentEvent.");
                            var requiredPaymentEvent =
                                requiredPaymentEventFactory.Create(requiredPayment.EarningType, group.Key);
                            mapper.Map(identifiedRemovedLearningAim, requiredPaymentEvent);
                            mapper.Map(requiredPayment, requiredPaymentEvent);
                            var historicPayment = cacheItem.Value.FirstOrDefault(payment =>
                                payment.PriceEpisodeIdentifier == requiredPayment.PriceEpisodeIdentifier);
                            mapper.Map(historicPayment, requiredPaymentEvent);
                            if (historicPayment == null)
                                throw new InvalidOperationException($"Cannot find historic payment with price episode identifier: {requiredPayment.PriceEpisodeIdentifier}.");
                            logger.LogDebug($"Finished mapping ]");
                            return requiredPaymentEvent;
                        })
                )
                .ToList();
            return requiredPaymentEvents.AsReadOnly();
        }

        private List<RequiredPayment> RefundPayments(List<Payment> historicPayments)
        {
            return refundRemovedLearningAimService.RefundLearningAim(historicPayments);
        }
    }
}