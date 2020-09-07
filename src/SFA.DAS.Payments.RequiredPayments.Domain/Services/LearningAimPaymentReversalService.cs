using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class LearningAimPaymentReversalService : IRefundRemovedLearningAimService
    {

        public LearningAimPaymentReversalService()
        {
        }

        public List<(byte deliveryPeriod, RequiredPayment payment)> RefundLearningAim(List<Payment> historicPayments)
        {
            var deliveryPeriods = historicPayments.GroupBy(p => p.DeliveryPeriod).OrderBy(g => g.Key).ToList();
            var paymentsToReverse = new List<Payment>();
            foreach (var deliveryPeriodPayments in deliveryPeriods)
            {
                var refunds = deliveryPeriodPayments.Where(p => p.Amount < 0).ToList();
                foreach (var deliveryPeriodPayment in deliveryPeriodPayments.Where(p => p.Amount > 0))
                {
                    var matchingRefund = refunds.FirstOrDefault(refund =>
                        deliveryPeriodPayment.Amount == refund.Amount * -1 &&
                        deliveryPeriodPayment.TransactionType == refund.TransactionType &&
                        deliveryPeriodPayment.FundingSource == refund.FundingSource &&
                        deliveryPeriodPayment.PriceEpisodeIdentifier == refund.PriceEpisodeIdentifier &&
                        deliveryPeriodPayment.ApprenticeshipId == refund.ApprenticeshipId &&
                        deliveryPeriodPayment.SfaContributionPercentage == refund.SfaContributionPercentage);
                    if (matchingRefund != null)
                        refunds.Remove(matchingRefund);
                    else
                        paymentsToReverse.Add(deliveryPeriodPayment);
                }
                paymentsToReverse.AddRange(refunds);
            }

            return paymentsToReverse.Select(payment => (payment.DeliveryPeriod, new RequiredPayment
            {
                ApprenticeshipId = payment.ApprenticeshipId,
                AccountId = payment.AccountId,
                ApprenticeshipEmployerType = payment.ApprenticeshipEmployerType,
                ApprenticeshipPriceEpisodeId = payment.ApprenticeshipPriceEpisodeId,
                PriceEpisodeIdentifier = payment.PriceEpisodeIdentifier,
                TransferSenderAccountId = payment.TransferSenderAccountId,
                LearningStartDate = payment.LearningStartDate,
                SfaContributionPercentage = payment.SfaContributionPercentage,
                Amount = payment.Amount * -1,
                EarningType = GetEarningType(payment.FundingSource)
            })).ToList();
        }

        private EarningType GetEarningType(FundingSourceType fundingSource)
        {
            switch (fundingSource)
            {
                case FundingSourceType.Transfer:
                case FundingSourceType.Levy:
                    return EarningType.Levy;
                case FundingSourceType.CoInvestedEmployer:
                case FundingSourceType.CoInvestedSfa:
                    return EarningType.CoInvested;
                case FundingSourceType.FullyFundedSfa:
                    return EarningType.Incentive;
                default: 
                    throw new InvalidOperationException($"Cannot convert funding source {fundingSource:G} to earning type.");
            }
        }
    }
}