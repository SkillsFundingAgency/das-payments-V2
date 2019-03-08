using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class RefundService
    {
        public List<RequiredPayment> GetRefund(decimal amount, List<Payment> paymentHistory)
        {
            var result = new List<RequiredPayment>();

            if (amount >= 0)
            {
                return result;
            }

            var totalPaidInHistory = paymentHistory.Sum(x => x.Amount);
            if (totalPaidInHistory <= 0)
            {
                return result;
            }

            var totalRefundPercent = (-1 * amount) / totalPaidInHistory; // Earning amount is a negative

            result.AddRange(RefundsBySfaContributionPercentage(
                paymentHistory.Where(x => x.FundingSource == FundingSourceType.Levy), totalRefundPercent, EarningType.Levy));

            var coInvestedHistory = paymentHistory
                .Where(x => x.FundingSource == FundingSourceType.CoInvestedEmployer ||
                            x.FundingSource == FundingSourceType.CoInvestedSfa);
            result.AddRange(RefundsBySfaContributionPercentage(
                coInvestedHistory, totalRefundPercent, EarningType.CoInvested));

            result.AddRange(RefundsBySfaContributionPercentage(
                paymentHistory.Where(x => x.FundingSource == FundingSourceType.FullyFundedSfa), totalRefundPercent, EarningType.Incentive));

            return result;
        }

        List<RequiredPayment> RefundsBySfaContributionPercentage(IEnumerable<Payment> historyForType, decimal refundPercent, EarningType refundType)
        {
            var requiredPaymentsForType = new List<RequiredPayment>();
            var historyAsAList = historyForType.ToList();

            var totalPaidForType = historyAsAList.Sum(x => x.Amount);
            var refundForType = totalPaidForType * refundPercent;
            if (refundForType > 0)
            {
                var byContribution = historyAsAList.GroupBy(x => x.SfaContributionPercentage);
                foreach (var contribution in byContribution)
                {
                    var amount = -1 * contribution.Sum(x => x.Amount) * refundPercent; // Change amount back to a negative
                    requiredPaymentsForType.Add(new RequiredPayment
                    {
                        Amount = amount,
                        SfaContributionPercentage = contribution.Key,
                        EarningType = refundType,
                    });
                }
            }

            return requiredPaymentsForType;
        }
    }
}
