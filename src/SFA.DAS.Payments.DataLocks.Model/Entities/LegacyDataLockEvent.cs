using System;

namespace SFA.DAS.Payments.DataLocks.Model.Entities
{
    public class LegacyDataLockEvent
    {
        public Guid DataLockEventId { get; set; }
        public DateTime ProcessDateTime { get; set; }
        public string IlrFileName { get; set; }
        public DateTime SubmittedDateTime { get; set; }
        public string AcademicYear { get; set; }
        public long UKPRN { get; set; }
        public long ULN { get; set; }
        public string LearnRefNumber { get; set; }
        public long AimSeqNumber { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public long CommitmentId { get; set; }
        public long EmployerAccountId { get; set; }
        public int EventSource { get; set; }
        public bool HasErrors { get; set; }
        public DateTime? IlrStartDate { get; set; }
        public long? IlrStandardCode { get; set; }
        public int? IlrProgrammeType { get; set; }
        public int? IlrFrameworkCode { get; set; }
        public int? IlrPathwayCode { get; set; }
        public decimal? IlrTrainingPrice { get; set; }
        public decimal? IlrEndpointAssessorPrice { get; set; }
        public DateTime? IlrPriceEffectiveFromDate { get; set; }
        public DateTime? IlrPriceEffectiveToDate { get; set; }
    }
}