using System;
using System.Collections.Generic;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class RequiredPaymentProcessor : IRequiredPaymentProcessor
    {
        private IPaymentDueProcessor paymentsDue;
        private IRefundService refunds;

        public RequiredPaymentProcessor(IPaymentDueProcessor paymentsDue, IRefundService refunds)
        {
            this.paymentsDue = paymentsDue;
            this.refunds = refunds;
        }

        public List<RequiredPayment> GetRequiredPayments(Earning earning, List<Payment> paymentHistory)
        {
            var amount = string.IsNullOrWhiteSpace(earning.PriceEpisodeIdentifier) && earning.EarningType != EarningType.Incentive 
                ? 0M
                : paymentsDue.CalculateRequiredPaymentAmount(earning.Amount, paymentHistory);

            if (amount < 0)
            {
                return refunds.GetRefund(amount, paymentHistory);
            }

            if (amount == 0)
            {
                return new List<RequiredPayment>();
            }

            if (!earning.SfaContributionPercentage.HasValue)
            {
                throw new ArgumentException("Trying to use a null SFA Contribution % for a positive earning");
            }

            return new List<RequiredPayment>
            {
                new RequiredPayment
                {
                    Amount = amount,
                    EarningType = earning.EarningType,
                    SfaContributionPercentage = earning.SfaContributionPercentage.Value,
                    PriceEpisodeIdentifier = earning.PriceEpisodeIdentifier,
                },
            };
        }
    }
}
