using System.Collections.Generic;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface IPaymentDueProcessor
    {
        decimal CalculateRequiredPaymentAmount(decimal amountDue, IEnumerable<Payment> paymentHistory);
        decimal CalculateSfaContributionPercentage(decimal earningPercentage, decimal earningAmount, Payment[] paymentHistory);
    }
}