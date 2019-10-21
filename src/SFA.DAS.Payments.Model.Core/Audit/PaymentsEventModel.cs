using System;

namespace SFA.DAS.Payments.Model.Core.Audit
{
    public interface IPaymentsEventModel
    {
        Guid EventId { get; set; }
        byte CollectionPeriod { get; set; }
        string LearnerReferenceNumber { get; set; }
        long LearnerUln { get; set; }
        string LearningAimReference { get; set; }
        int LearningAimProgrammeType { get; set; }
        int LearningAimStandardCode { get; set; }
        int LearningAimFrameworkCode { get; set; }
        int LearningAimPathwayCode { get; set; }
        string LearningAimFundingLineType { get; set; }
        long Ukprn { get; set; }
        DateTime IlrSubmissionDateTime { get; set; }
        long JobId { get; set; }
        DateTimeOffset EventTime { get; set; }
        short AcademicYear { get; set; }
        DateTime StartDate { get; set; }
        DateTime? PlannedEndDate { get; set; }
        DateTime? ActualEndDate { get; set; }
        byte CompletionStatus { get; set; }
        decimal CompletionAmount { get; set; }
        decimal InstalmentAmount { get; set; }
        short NumberOfInstalments { get; set; }
        DateTime? LearningStartDate { get; set; }
    }

    public abstract class PaymentsEventModel : IPaymentsEventModel
    {
        public Guid EventId { get; set; }
        public short AcademicYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public byte CompletionStatus { get; set; }
        public decimal CompletionAmount { get; set; }
        public decimal InstalmentAmount { get; set; }
        public short NumberOfInstalments { get; set; }
        public DateTime? LearningStartDate { get; set; }
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
       
    }
}