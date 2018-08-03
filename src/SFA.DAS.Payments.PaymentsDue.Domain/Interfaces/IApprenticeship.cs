using System.Collections.Generic;
using SFA.DAS.Payments.PaymentsDue.Domain.Entities;

namespace SFA.DAS.Payments.PaymentsDue.Domain.Interfaces
{
    public interface IApprenticeship
    {
        Course Course { get; set; }

        Learner Learner { get; set; }

        long Ukprn { get; set; }

        string Key { get; }

        IEnumerable<PaymentDue> CreatePaymentDue(IEnumerable<PayableEarning> earnings, IEnumerable<Payment> paymentHistory);
    }
}