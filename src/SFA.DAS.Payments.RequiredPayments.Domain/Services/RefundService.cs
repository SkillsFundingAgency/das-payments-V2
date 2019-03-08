using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class RefundService
    {
        public List<RequiredPayment> GetRefund(Earning earning, List<Payment> paymentHistory)
        {
            var result = new List<RequiredPayment>();

            if (earning.Amount >= 0)
            {
                return result;
            }

            var totalPaidInHistory = paymentHistory.Sum(x => x.Amount);
            if (totalPaidInHistory <= 0)
            {
                return result;
            }

            var paymentsBySource = paymentHistory.ToLookup(x => x.FundingSource);

            var totalPaidForLevy = paymentHistory.Where(x => x.FundingSource == FundingSourceType.Levy).Sum(x => x.Amount);
            var totalPaidForEmployer = paymentHistory.Where(x => x.FundingSource == FundingSourceType.CoInvestedEmployer).Sum(x => x.Amount);
            var totalPaidForProvider = paymentHistory.Where(x => x.FundingSource == FundingSourceType.CoInvestedSfa).Sum(x => x.Amount);
            var totalPaidForIncentives = paymentHistory.Where(x => x.FundingSource == FundingSourceType.FullyFundedSfa).Sum(x => x.Amount);

            var totalRefundPercent = (-1 * earning.Amount) / totalPaidInHistory; // Earning amount is a negative

            var levyRefund = totalPaidForLevy * totalRefundPercent;
            if (levyRefund > 0)
            {
                result.Add(new RequiredPayment
                {
                    EarningType = EarningType.Levy,
                    Amount = -1 * levyRefund,
                    SfaContributionPercentage = earning.SfaContributionPercentage,
                });
            }

            var incentiveRefund = totalPaidForIncentives * totalRefundPercent;
            if (incentiveRefund > 0)
            {
                result.Add(new RequiredPayment
                {
                    EarningType = EarningType.Incentive,
                    Amount = -1 * incentiveRefund,
                    SfaContributionPercentage = earning.SfaContributionPercentage,
                });
            }

            //var coInvestedRefund = 

            return result;
        }
    }
}
