using System;
using DCT.TestDataGenerator;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    /// <summary>
    /// Class Representing a Learner Request
    /// </summary>
    public class LearnerRequest
    {
        public int Ukprn { get; set; }

        public string FeatureNumber { get; set; }

        public string LearnerId { get; set; }

        public long Uln { get; set; }

        public DateTime? StartDate { get; set; }

        public int? PlannedDurationInMonths { get; set; }

        public int? TotalTrainingPrice { get; set; }

        public DateTime? TotalTrainingPriceEffectiveDate { get; set; }

        public int? TotalAssessmentPrice { get; set; }

        public DateTime? TotalAssessmentPriceEffectiveDate { get; set; }

        public int? ActualDurationInMonths { get; set; }

        public CompStatus? CompletionStatus { get; set; }

        public int? AimSequenceNumber { get; set; }

        public string AimReferenceNumber { get; set; }

        public int? FrameworkCode { get; set; }

        public int? PathwayCode { get; set; }

        public int? ProgrammeType { get; set; }

        public string FundingLineType { get; set; }

        public string SfaContributionPercentage { get; set; }

        public ContractType ContractType { get; set; }
    }
}
