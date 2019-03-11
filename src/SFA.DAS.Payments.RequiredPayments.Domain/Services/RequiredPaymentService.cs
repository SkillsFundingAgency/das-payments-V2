using System.Collections.Generic;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class RequiredPaymentService
    {
        private IPaymentDueProcessor paymentsDue;
        private IRefundService refunds;

        public RequiredPaymentService(IPaymentDueProcessor paymentsDue, IRefundService refunds)
        {
            this.paymentsDue = paymentsDue;
            this.refunds = refunds;
        }

        public List<RequiredPayment> GetRequiredPayments(Earning earning, List<Payment> paymentHistory)
        {
            var amount = paymentsDue.CalculateRequiredPaymentAmount(earning.Amount, paymentHistory);
            if (amount > 0)
            {
                return new List<RequiredPayment>
                {
                    new RequiredPayment
                    {
                        Amount = amount,
                        EarningType = earning.EarningType,
                        SfaContributionPercentage = earning.SfaContributionPercentage,
                    },
                };
            }
            return refunds.GetRefund(amount, paymentHistory);
        }
    }
}
