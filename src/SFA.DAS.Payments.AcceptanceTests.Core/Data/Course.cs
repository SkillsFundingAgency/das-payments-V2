using System;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class Course
    {
        public short AimSeqNumber { get; set; }
        public int ProgrammeType { get; set; }
        public int FrameworkCode { get; set; }
        public int PathwayCode { get; set; }
        public int StandardCode { get; set; }
        public string FundingLineType { get; set; }
        public string LearnAimRef { get; set; }
        public DateTime LearningStartDate { get; set; }
        public DateTime? LearningPlannedEndDate { get; set; }
        public DateTime? LearningActualEndDate { get; set; }
        public string CompletionStatus { get; set; }
        public decimal AgreedPrice { get; set; }
    }
}
