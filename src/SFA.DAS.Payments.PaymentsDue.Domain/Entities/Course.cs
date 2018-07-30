using SFA.DAS.Payments.PaymentsDue.Domain.Enums;

namespace SFA.DAS.Payments.PaymentsDue.Domain.Entities
{
    public class Course
    {
        public ProgrammeType ProgrammeType { get; set; }

        public int StandardCode { get; set; }

        public int FrameworkCode { get; set; }

        public int PathwayCode { get; set; }
    }
}
