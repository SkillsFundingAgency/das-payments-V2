using System;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class SubmittedLearnerAimModel
    {
        public long Id { get; set; }
        public long Ukprn { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public int LearningAimFrameworkCode { get; set; }
        public int LearningAimPathwayCode { get; set; }
        public int LearningAimProgrammeType { get; set; }
        public int LearningAimStandardCode { get; set; }
        public string LearningAimReference { get; set; }
        public byte CollectionPeriod { get; set; }
        public short AcademicYear { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public long LearnerUln { get; set; }
        public long JobId { get; set; }
    }
}
