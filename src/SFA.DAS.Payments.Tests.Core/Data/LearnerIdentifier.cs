namespace SFA.DAS.Payments.Tests.Core.Data
{
    public abstract class LearnerIdentifier
    {
        public string LearnRefNumber { get; set; }

        public long Ukprn { get; set; }
    }
}