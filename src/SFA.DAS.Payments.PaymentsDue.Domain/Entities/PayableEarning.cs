﻿namespace SFA.DAS.Payments.PaymentsDue.Domain.Entities
{
    public class PayableEarning
    {
        public long Ukprn { get; set; }
        
        public Learner Learner { get;set; }

        public Course Course { get; set; }

        string LearnAimRef { get; set; }
    }
}
