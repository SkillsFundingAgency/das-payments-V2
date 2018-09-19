using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.RequiredPayments.Domain.Interfaces;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Entities
{
    public class Apprenticeship : IApprenticeship
    {
        public long Ukprn { get; set; }

        public Learner Learner { get; set; }

        public Course Course { get; set; }

        public IEnumerable<PaymentDue> CreatePaymentDue(IEnumerable<PayableEarning> earnings, IEnumerable<Payment> paymentHistory)
        {
            return earnings.Select(e =>
                new PaymentDue
                {
                    Earning = new PayableEarning
                    {
                        Ukprn = e.Ukprn,
                        Learner = new Learner
                        {
                            LearnerReferenceNumber = e.Learner.LearnerReferenceNumber,
                            Ukprn = e.Ukprn,
                            Uln = e.Learner.Uln
                        },
                        Course = new Course
                        {
                            ProgrammeType = e.Course.ProgrammeType,
                            PathwayCode = e.Course.PathwayCode,
                            StandardCode = e.Course.StandardCode,
                            FrameworkCode = e.Course.FrameworkCode,
                            LearnAimRef = e.Course.LearnAimRef
                        }                        
                    }
                }).ToArray();
        }
    }
}
