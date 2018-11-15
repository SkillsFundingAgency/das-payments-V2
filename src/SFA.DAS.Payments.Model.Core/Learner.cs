namespace SFA.DAS.Payments.Model.Core
{
    public class Learner
    {
        public string ReferenceNumber { get; set; }
        public long Uln { get; set; }
        public Learner Clone()
        {
            return (Learner) MemberwiseClone();
        }
    }
}
