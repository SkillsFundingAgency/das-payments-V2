using System.Collections.Generic;
using System.Globalization;
using SFA.DAS.Payments.PaymentsDue.Domain.Interfaces;

namespace SFA.DAS.Payments.PaymentsDue.Domain.Entities
{
    public class Apprenticeship : IApprenticeship
    {
        public long Ukprn { get; set; }

        public Learner Learner { get; set; }

        public Course Course { get; set; }

        public IEnumerable<PaymentDue> CreatePaymentDue(IEnumerable<PayableEarning> earnings, IEnumerable<Payment> paymentHistory)
        {
            return new PaymentDue[0];
        }
    }
}
