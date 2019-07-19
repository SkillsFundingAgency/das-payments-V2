using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.EarningEvents.Model.Entities
{
    public class SubmittedPriceEpisodeEntity
    {
        public long Id { get; set; }
        public long Ukprn { get; set; } // ilr, have
        public string LearnRefNumber { get; set; } // aim, have
        public string PriceEpisodeIdentifier { get; set; } // multiple per aim, have
        public IlrDetails IlrDetails { get; set; }
    }

    public class IlrDetails
    {
        public string IlrFileName { get; set; } // ilr, can have
        public DateTime FileDateTime { get; set; } // ilr, ?
        public DateTime SubmittedDateTime { get; set; } // ilr, have
        public int ComponentVersionNumber { get; set; } // hardcoded to 1 in v1. Set it to 2
        public long Uln { get; set; } // aim, have
        public long AimSeqNumber { get; set; } // aim, have
        public long? StandardCode { get; set; } // aim, have
        public int? ProgrammeType { get; set; } // aim, have
        public int? FrameworkCode { get; set; } // aim, have
        public int? PathwayCode { get; set; } // aim have
        public DateTime ActualStartDate { get; set; } // aim, have
        public DateTime PlannedEndDate { get; set; } // aim, have 
        public DateTime? ActualEndDate { get; set; } // aim, have       
        public decimal OnProgrammeTotalPrice { get; set; } //aim, have: CASE WHEN ISNULL(pe.TNP1,0) > 0 THEN pe.TNP1 ELSE pe.TNP3 END OnProgrammeTotalPrice,        
        public decimal CompletionTotalPrice { get; set; } //aim, CASE WHEN ISNULL(pe.TNP1,0) > 0 THEN pe.TNP2 ELSE pe.TNP4 END CompletionTotalPrice,

        public string AcademicYear { get; set; } // ilr, have
        public long? CommitmentId { get; set; } // aim, have

        public int EmployerReferenceNumber { get; set; } // aim, don't have. Present in ILR Learner/LearnerEmploymentStatus/EmpId
        public int? CompStatus { get; set; } // aim, don't have. Present in ILR Learner/LearningDelivery/CompStatus        
        public string NiNumber { get; set; } // aim, don't have. Present in ILR
        public string GivenNames { get; set; } // aim, don't have. Present in ILR
        public string FamilyName { get; set; } // aim, don't have. Present in ILR
        public string EPAOrgId { get; set; } // aim, ?
    }
}