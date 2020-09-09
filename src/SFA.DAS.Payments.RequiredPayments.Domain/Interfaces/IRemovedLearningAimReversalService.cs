using System.Collections.Generic;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface IRemovedLearningAimReversalService
    {
        List<(byte deliveryPeriod, RequiredPayment payment)> RefundLearningAim(List<Payment> historicPayments);
    }
}