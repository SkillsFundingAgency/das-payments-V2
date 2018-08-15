using SFA.DAS.Payments.RequiredPayments.Domain.Enums;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Entities
{
    public class Course
    {
        public ProgrammeType ProgrammeType { get; set; }

        public int StandardCode { get; set; }

        public int FrameworkCode { get; set; }

        public int PathwayCode { get; set; }

        public string LearnAimRef { get; set; }
    }
}
