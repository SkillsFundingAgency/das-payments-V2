using System.Collections.Generic;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface IRefundRemovedLearningAimService
    {
        List<RequiredPayment> RefundLearningAim(List<Payment> historicPayments);
    }
}