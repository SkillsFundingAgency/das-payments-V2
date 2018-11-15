namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class Learner
    {
        public long Ukprn { get; set; }
        public string LearnRefNumber { get; set; }
        public long Uln { get; set; }
        public Course Course { get; set; }

        public string LearnerIdentifier { get; set; }
    }
}