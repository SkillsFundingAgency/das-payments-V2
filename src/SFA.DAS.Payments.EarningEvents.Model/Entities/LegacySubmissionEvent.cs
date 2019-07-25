using System;

namespace SFA.DAS.Payments.EarningEvents.Model.Entities
{
    public class LegacySubmissionEvent
    {
        public string IlrFileName { get; set; }
        public DateTime FileDateTime { get; set; }
        public DateTime SubmittedDateTime { get; set; }
        public int ComponentVersionNumber { get; set; }
        public long UKPRN { get; set; }
        public long ULN { get; set; }
        public string LearnRefNumber { get; set; }
        public long AimSeqNumber { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public long? StandardCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? FrameworkCode { get; set; }
        public int? PathwayCode { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal? OnProgrammeTotalPrice { get; set; }
        public decimal? CompletionTotalPrice { get; set; }
        public string NINumber { get; set; }
        public long? CommitmentId { get; set; }

        public int EmployerReferenceNumber { get; set; }
        public string AcademicYear { get; set; }
        public string EPAOrgId { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public int? CompStatus { get; set; }

    }
}
