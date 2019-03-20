using System.Collections.Generic;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface IRequiredPaymentProcessor
    {
        List<RequiredPayment> GetRequiredPayments(Earning earning, List<Payment> paymentHistory);
    }
}