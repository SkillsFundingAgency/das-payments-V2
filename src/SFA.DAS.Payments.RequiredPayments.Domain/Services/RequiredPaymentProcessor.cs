using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class RequiredPaymentProcessor : IRequiredPaymentProcessor
    {
        private readonly IPaymentDueProcessor paymentsDue;
        private readonly IRefundService refundService;

        public RequiredPaymentProcessor(IPaymentDueProcessor paymentsDue, IRefundService refundService)
        {
            this.paymentsDue = paymentsDue;
            this.refundService = refundService;
        }

        public List<RequiredPayment> GetRequiredPayments(Earning earning, List<Payment> paymentHistory)
        {
             var result = new List<RequiredPayment>();
            if (earning.EarningType != EarningType.Incentive && earning.SfaContributionPercentage.HasValue)
                result.AddRange(RefundPaymentsWithDifferentSfaContribution(earning.SfaContributionPercentage.Value, paymentHistory));

            var validPaymentHistory = paymentHistory
                .Where(p => earning.EarningType == EarningType.Incentive || !earning.SfaContributionPercentage.HasValue || p.SfaContributionPercentage == earning.SfaContributionPercentage)
                .ToList();

            var amount = string.IsNullOrWhiteSpace(earning.PriceEpisodeIdentifier) && earning.EarningType != EarningType.Incentive
                ? 0M
                : paymentsDue.CalculateRequiredPaymentAmount(earning.Amount, validPaymentHistory);

            if (amount < 0)
            {
                result.AddRange(refundService.GetRefund(amount, validPaymentHistory));
                return result;
            }

            if (amount == 0)
            {
                return result;
            }

            if (!earning.SfaContributionPercentage.HasValue)
            {
                throw new ArgumentException("Trying to use a null SFA Contribution % for a positive earning");
            }

            result.Add(new RequiredPayment
            {
                Amount = amount,
                EarningType = earning.EarningType,
                SfaContributionPercentage = earning.SfaContributionPercentage.Value,
                PriceEpisodeIdentifier = earning.PriceEpisodeIdentifier,
            });

            return result;
        }

        private List<RequiredPayment> RefundPaymentsWithDifferentSfaContribution(decimal currentSfaContributionPercentage, List<Payment> paymentHistory)
        {
            return paymentHistory
                .Where(payment => payment.SfaContributionPercentage != currentSfaContributionPercentage)
                .GroupBy(payment => payment.SfaContributionPercentage)
                .SelectMany(group => refundService.GetRefund(0, group.ToList()))
                .ToList();
        }
    }
}
