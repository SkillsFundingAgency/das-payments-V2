namespace SFA.DAS.Payments.RequiredPayments.Domain.Entities
{
    public class Learner
    {
        public long Ukprn { get; set; }

        public string LearnerReferenceNumber { get; set; }

        public long Uln { get; set; }

        public bool IsTemp => Uln == 9999999;
    }
}
