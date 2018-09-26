using System;
using System.Linq;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class ApprenticeshipContractType2PaymentDueProcessor : IApprenticeshipContractType2PaymentDueProcessor
    {
        public decimal CalculateRequiredPaymentAmount(decimal amountDue, Payment[] paymentHistory)
        {
            if (paymentHistory == null)
                throw new ArgumentNullException(nameof(paymentHistory));

            return amountDue - paymentHistory.Sum(p => p.Amount);
        }
    }
}
