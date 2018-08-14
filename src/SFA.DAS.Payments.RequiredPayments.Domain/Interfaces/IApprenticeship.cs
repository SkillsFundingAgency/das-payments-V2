using System.Collections.Generic;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Interfaces
{
    public interface IApprenticeship
    {
        Course Course { get; set; }

        Learner Learner { get; set; }

        long Ukprn { get; set; }

        IEnumerable<PaymentDue> CreatePaymentDue(IEnumerable<PayableEarning> earnings, IEnumerable<Payment> paymentHistory);
    }
}