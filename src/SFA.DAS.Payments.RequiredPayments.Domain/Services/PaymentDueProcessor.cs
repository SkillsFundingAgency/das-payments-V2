using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
