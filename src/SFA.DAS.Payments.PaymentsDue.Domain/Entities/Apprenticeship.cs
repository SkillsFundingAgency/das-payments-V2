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

        public string Key
        {
            get
            {
                return string.Join("-", 
                    new[]
                    {
                        Ukprn.ToString(CultureInfo.InvariantCulture),
                        Learner.LearnerReferenceNumber,
                        Course.FrameworkCode.ToString(CultureInfo.InvariantCulture),
                        Course.PathwayCode.ToString(CultureInfo.InvariantCulture),
                        ((int)Course.ProgrammeType).ToString(CultureInfo.InvariantCulture),
                        Course.StandardCode.ToString(CultureInfo.InvariantCulture),
                        Course.LearnAimRef.ToString(CultureInfo.InvariantCulture)
                    }
                );
            }
        }

        public IEnumerable<PaymentDue> CreatePaymentDue(IEnumerable<PayableEarning> earnings, IEnumerable<Payment> paymentHistory)
        {
            return new PaymentDue[0];
        }
    }
}
