using System.Collections.Generic;
using SFA.DAS.Payments.PaymentsDue.Domain.Models;

namespace SFA.DAS.Payments.PaymentsDue.Domain.Entities
{
    public class Apprenticeship : IApprenticeship
    {
        public long Ukprn { get; set; }

        public Learner Learner { get; set; }

        public Course Course { get; set; }

        public IEnumerable<HistoricalPayment> GetPaymentHistory()
        {
            return new HistoricalPayment[0];
        }

        public IEnumerable<PaymentDue> CreatePaymentDue(IEnumerable<PayableEarning> earnings, IEnumerable<HistoricalPayment> paymentHistory)
        {
            return new PaymentDue[0];
        }
    }
}
