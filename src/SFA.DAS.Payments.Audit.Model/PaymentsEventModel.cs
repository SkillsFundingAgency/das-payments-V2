using System;

namespace SFA.DAS.Payments.Audit.Model
{
    public abstract class PaymentsEventModel
    {
        public Guid EventId { get; set; }
        public byte CollectionPeriod { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public long LearnerUln { get; set; }
        public string LearningAimReference { get; set; }
        public int LearningAimProgrammeType { get; set; }
        public int LearningAimStandardCode { get; set; }
        public int LearningAimFrameworkCode { get; set; }
        public int LearningAimPathwayCode { get; set; }
        public string LearningAimFundingLineType { get; set; }
        public long Ukprn { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public long JobId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public string CollectionYear { get; set; }
    }
}