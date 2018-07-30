using System.Collections.Generic;
using SFA.DAS.Payments.PaymentsDue.Domain.Entities;

namespace SFA.DAS.Payments.PaymentsDue.Domain.Models
{
    public interface IApprenticeship
    {
        Course Course { get; set; }
        Learner Learner { get; set; }
        long Ukprn { get; set; }

        IEnumerable<PaymentDue> CreatePaymentDue(IEnumerable<PayableEarning> earnings, IEnumerable<HistoricalPayment> paymentHistory);
        IEnumerable<HistoricalPayment> GetPaymentHistory();
    }
}