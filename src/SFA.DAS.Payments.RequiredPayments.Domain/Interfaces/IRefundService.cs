using System.Collections.Generic;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface IRefundService
    {
        List<RequiredPayment> GetRefund(decimal amount, List<Payment> paymentHistory);
    }
}