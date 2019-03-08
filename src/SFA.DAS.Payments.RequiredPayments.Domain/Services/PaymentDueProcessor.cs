using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class PaymentDueProcessor : IPaymentDueProcessor
    {
        public decimal CalculateRequiredPaymentAmount(decimal amountDue, IEnumerable<Payment> paymentHistory)
        {
            if (paymentHistory == null)
                throw new ArgumentNullException(nameof(paymentHistory));

            return amountDue - paymentHistory.Sum(p => p.Amount);
        }

        public decimal CalculateSfaContributionPercentage(decimal earningPercentage, decimal earningAmount, Payment[] paymentHistory)
        {
            if (earningPercentage == 0 && earningAmount == 0 && paymentHistory.Length > 0)
            {
                var sfaContribution = paymentHistory.Where(p => p.FundingSource == FundingSourceType.CoInvestedSfa).Sum(p => p.Amount);
                var employerContribution = paymentHistory.Where(p => p.FundingSource == FundingSourceType.CoInvestedEmployer).Sum(p => p.Amount);
                if (sfaContribution + employerContribution == 0) // protection from div by 0
                {
                    earningPercentage = 0;
                }
                else
                {
                    earningPercentage = sfaContribution / (sfaContribution + employerContribution);
                }
            }

            return earningPercentage;
        }
    }
}
