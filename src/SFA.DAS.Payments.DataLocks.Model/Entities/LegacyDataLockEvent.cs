using System;

namespace SFA.DAS.Payments.DataLocks.Model.Entities
{
    public class LegacyDataLockEvent
    {
        public DateTime ProcessDateTime { get; set; }
        public int Status { get; set; }
        public string IlrFileName { get; set; }
        public DateTime SubmittedDateTime { get; set; }
        public string AcademicYear { get; set; }
        public long Ukprn { get; set; }
        public long Uln { get; set; }
        public string LearnRefnumber { get; set; }
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
        public Guid DataLockEventId { get; set; }
    }
}