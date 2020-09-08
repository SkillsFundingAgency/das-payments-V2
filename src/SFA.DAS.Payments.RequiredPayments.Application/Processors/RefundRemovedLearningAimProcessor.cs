using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
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
        private readonly IDuplicateEarningEventService duplicateEarningEventService;

        public RefundRemovedLearningAimProcessor(IRefundRemovedLearningAimService refundRemovedLearningAimService,
            IPaymentLogger logger, IMapper mapper, IPeriodisedRequiredPaymentEventFactory requiredPaymentEventFactory,
            IDuplicateEarningEventService duplicateEarningEventService
        )
        {
            this.refundRemovedLearningAimService = refundRemovedLearningAimService ?? throw new ArgumentNullException(nameof(refundRemovedLearningAimService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.requiredPaymentEventFactory = requiredPaymentEventFactory ?? throw new ArgumentNullException(nameof(requiredPaymentEventFactory));
            this.duplicateEarningEventService = duplicateEarningEventService ?? throw new ArgumentNullException(nameof(duplicateEarningEventService));
        }

        public async Task<ReadOnlyCollection<PeriodisedRequiredPaymentEvent>> RefundLearningAim(IdentifiedRemovedLearningAim identifiedRemovedLearningAim, IDataCache<PaymentHistoryEntity[]> paymentHistoryCache, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Now processing request to generate refunds for learning aim: learner: {identifiedRemovedLearningAim.Learner.ReferenceNumber}, Aim: {identifiedRemovedLearningAim.LearningAim.Reference}");
            if (await duplicateEarningEventService.IsDuplicate(identifiedRemovedLearningAim, cancellationToken))
            {
                logger.LogWarning($"Duplicate Identified Removed Learning Aim found for learner with JobId: {identifiedRemovedLearningAim.JobId}, " +
                                  $"Learner Ref Number: {identifiedRemovedLearningAim.Learner.ReferenceNumber}, Aim: {identifiedRemovedLearningAim.LearningAim.Reference}");
                return new List<PeriodisedRequiredPaymentEvent>().AsReadOnly();
            }

            var cacheItem = await paymentHistoryCache.TryGet(CacheKeys.PaymentHistoryKey, cancellationToken)
                .ConfigureAwait(false);
            if (!cacheItem.HasValue)
                throw new InvalidOperationException("No payment history found in the cache.");

            var historicPayments = cacheItem.Value.Select(mapper.Map<PaymentHistoryEntity, Payment>).ToList();
            logger.LogDebug($"Got {historicPayments.Count} historic payments. Now generating refunds per transaction type.");

            var requiredPaymentEvents = historicPayments.GroupBy(historicPayment => historicPayment.TransactionType)
                .SelectMany(group => CreateRefundPayments(identifiedRemovedLearningAim, group.ToList(), group.Key, cacheItem))
                .ToList();

            return requiredPaymentEvents.AsReadOnly();
        }

        private IList<PeriodisedRequiredPaymentEvent> CreateRefundPayments(IdentifiedRemovedLearningAim identifiedRemovedLearningAim, List<Payment> historicPaymentsByTransactionType, int transactionType, ConditionalValue<PaymentHistoryEntity[]> cacheItem)
        {
            var refundPaymentsAndPeriods = refundRemovedLearningAimService.RefundLearningAim(historicPaymentsByTransactionType);

            return refundPaymentsAndPeriods
                .Select(refund =>
                {
                    logger.LogVerbose("Now mapping the required payment to a PeriodisedRequiredPaymentEvent.");

                    var requiredPaymentEvent = requiredPaymentEventFactory.Create(refund.payment.EarningType, transactionType);
                    if (requiredPaymentEvent == null)
                    {
                        // This shouldn't now happen as the transaction type in the history should match the one in the cache
                        logger.LogWarning(
                            $"Required payment event is null for EarningType: {refund.payment.EarningType} with TransactionType: {transactionType}");
                        return null;
                    }

                    var reversedPayment = historicPaymentsByTransactionType.FirstOrDefault(p => p.Id == refund.payment.ReversedPaymentId);
                    if (reversedPayment == null)
                        throw new InvalidOperationException($"Failed to find the payment to be reversed.  Reversed payment id: {refund.payment.ReversedPaymentId}");

                    requiredPaymentEvent.DeliveryPeriod = refund.deliveryPeriod;
                    requiredPaymentEvent.ApprenticeshipId = refund.payment.ApprenticeshipId;
                    requiredPaymentEvent.AccountId = refund.payment.AccountId;
                    requiredPaymentEvent.ApprenticeshipEmployerType = refund.payment.ApprenticeshipEmployerType;
                    requiredPaymentEvent.ApprenticeshipPriceEpisodeId = refund.payment.ApprenticeshipPriceEpisodeId;
                    requiredPaymentEvent.PriceEpisodeIdentifier = refund.payment.PriceEpisodeIdentifier;
                    requiredPaymentEvent.TransferSenderAccountId = refund.payment.TransferSenderAccountId;
                    requiredPaymentEvent.LearningStartDate = refund.payment.LearningStartDate;
                    requiredPaymentEvent.AmountDue = refund.payment.Amount;

                    requiredPaymentEvent.Learner = identifiedRemovedLearningAim.Learner;
                    requiredPaymentEvent.LearningAim = new LearningAim
                    {
                        ProgrammeType = identifiedRemovedLearningAim.LearningAim.ProgrammeType,
                        FrameworkCode = identifiedRemovedLearningAim.LearningAim.FrameworkCode,
                        PathwayCode = identifiedRemovedLearningAim.LearningAim.PathwayCode,
                        StandardCode = identifiedRemovedLearningAim.LearningAim.StandardCode,
                        FundingLineType = reversedPayment.LearningAimFundingLineType,
                        Reference = identifiedRemovedLearningAim.LearningAim.Reference
                    };
                    requiredPaymentEvent.Ukprn = identifiedRemovedLearningAim.Ukprn;
                    requiredPaymentEvent.StartDate = reversedPayment.StartDate;
                    requiredPaymentEvent.AccountId = reversedPayment.AccountId;
                    requiredPaymentEvent.ActualEndDate = reversedPayment.ActualEndDate;
                    requiredPaymentEvent.CollectionPeriod = identifiedRemovedLearningAim.CollectionPeriod;
                    requiredPaymentEvent.CompletionAmount = reversedPayment.CompletionAmount;
                    requiredPaymentEvent.CompletionStatus = reversedPayment.CompletionStatus;
                    requiredPaymentEvent.ContractType = reversedPayment.ContractType;
                    requiredPaymentEvent.IlrSubmissionDateTime = identifiedRemovedLearningAim.IlrSubmissionDateTime;
                    requiredPaymentEvent.CompletionStatus = reversedPayment.CompletionStatus;
                    requiredPaymentEvent.InstalmentAmount = reversedPayment.InstalmentAmount;
                    requiredPaymentEvent.NumberOfInstalments = reversedPayment.NumberOfInstalments;
                    requiredPaymentEvent.JobId = identifiedRemovedLearningAim.JobId;
                    requiredPaymentEvent.ReportingAimFundingLineType = reversedPayment.ReportingAimFundingLineType;

                    switch (requiredPaymentEvent)
                    {
                        case CalculatedRequiredCoInvestedAmount coInvestedRequiredPaymentEvent:
                            coInvestedRequiredPaymentEvent.SfaContributionPercentage = reversedPayment.SfaContributionPercentage;
                            break;
                        case CalculatedRequiredLevyAmount levyRequiredPaymentEvent:
                            levyRequiredPaymentEvent.SfaContributionPercentage = reversedPayment.SfaContributionPercentage;
                            break;
                    }

                    logger.LogDebug("Finished mapping");
                    return requiredPaymentEvent;
                })
                .Where(x => x != null)
                .ToList();
        }



        private void Map(CalculatedRequiredCoInvestedAmount requiredPayment, Payment reversedPayment)
        {
            requiredPayment.SfaContributionPercentage = reversedPayment.SfaContributionPercentage;
        }

        private void Map(CalculatedRequiredIncentiveAmount requiredPayment, Payment reversedPayment)
        {
            //nothing to map
        }

        private void Map(CalculatedRequiredLevyAmount requiredPayment, Payment reversedPayment)
        {
            //            requiredPayment.AgreedOnDate = reversedPayment.AgreedOnDate;
            //            requiredPayment.AgreementId = reversedPayment.AgreementId;
        }
    }
}