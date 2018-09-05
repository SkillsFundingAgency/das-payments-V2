using System;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data
{
    public class Course
    {
        public short AimSeqNumber { get; set; }

        public short ProgrammeType { get; set; }

        public short FrameworkCode { get; set; }

        public short PathwayCode { get; set; }

        public int StandardCode { get; set; }

        public string FundingLineType { get; set; }

        public string LearnAimRef { get; set; }

        public DateTime LearningStartDate { get; set; }

        public DateTime? LearningPlannedEndDate { get; set; }

        public DateTime? LearningActualEndDate { get; set; }

        public string CompletionStatus { get; set; }

        public LearningAim AsLearningAim()
        {
            return new LearningAim
            {
                Reference = LearnAimRef,
                ProgrammeType = ProgrammeType,
                StandardCode = StandardCode,
                FrameworkCode = FrameworkCode,
                PathwayCode = PathwayCode,
                FundingLineType = FundingLineType
            };
        }
    }
}
