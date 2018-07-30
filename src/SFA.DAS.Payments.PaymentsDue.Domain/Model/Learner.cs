using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.PaymentsDue.Domain.Model
{
    public class Learner
    {
        public long Ukprn { get; set; }

        public string LearnerReferenceNumber { get; set; }

        public long Uln { get; set; }

        public bool IsTemp => Uln == 9999999;
    }
}
