using System;

namespace SFA.DAS.Payments.Model.Core
{
    public class LearningAim
    {
        public string Reference { get; set; }

        public int ProgrammeType { get; set; }

        public int StandardCode { get; set; }

        public int FrameworkCode { get; set; }

        public int PathwayCode { get; set; }

        public string FundingLineType { get; set; }

        public DateTime StartDate { get; set; }

        public LearningAim Clone()
        {
            return (LearningAim)MemberwiseClone();
        }
    }
}
