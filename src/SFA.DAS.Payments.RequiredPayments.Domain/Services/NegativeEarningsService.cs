using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class NegativeEarningsService : INegativeEarningService
    {
        private readonly IRefundService refundService;

        public NegativeEarningsService(IRefundService refundService)
        {
            this.refundService = refundService;
        }

        public List<RequiredPayment> ProcessNegativeEarning(decimal amount, List<Payment> paymentHistory, int deliveryPeriod, string priceEpisodeIdentifier)
        {
            var results = new List<RequiredPayment>();
            var amountLeftToFund = amount;

            for (var i = deliveryPeriod; i > 0; i--)
            {
                if (amountLeftToFund >= 0)
                {
                    break;
                }

                var paymentHistoryForPeriod = paymentHistory
                    .Where(x => x.DeliveryPeriod == i)
                    .ToList();

                var amountPaidInPeriod = paymentHistoryForPeriod.Sum(x => x.Amount);

                if (amountPaidInPeriod == 0)
                {
                    continue;
                }

                var paymentsForPeriod = refundService.GetRefund(amountLeftToFund, paymentHistoryForPeriod);

                foreach (var requiredPayment in paymentsForPeriod)
                {
                    requiredPayment.PriceEpisodeIdentifier = priceEpisodeIdentifier;
                }

                amountLeftToFund -= paymentsForPeriod.Sum(x => x.Amount);

                results.AddRange(paymentsForPeriod);
            }

            return results;
        }

        private EarningType ToEarningType(FundingSourceType fundingSource)
        {
            switch (fundingSource)
            {
                case FundingSourceType.Levy:
                    return EarningType.Levy;
                case FundingSourceType.CoInvestedEmployer:
                case FundingSourceType.CoInvestedSfa:
                    return EarningType.CoInvested;
                case FundingSourceType.FullyFundedSfa:
                    return EarningType.Incentive;
            }

            throw new NotImplementedException($"Unknown funding source: {fundingSource}");
        }
    }
}