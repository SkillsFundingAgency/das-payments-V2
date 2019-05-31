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

        public string SmallEmployer { get; set; }

        public string PostcodePrior { get; set; }

        public long EmployerContribution => Convert.ToInt64(decimal.Parse(TotalTrainingPrice.Value.ToString()) * (SfaContributionPercentage.AsPercentage() / 100));
    }

    public static class LearnerRequestExtension
    {
        public static int ToLearnerAge(this string fundingLineType)
        {
            switch (fundingLineType)
            {
                case "16-18 Apprenticeship (From May 2017) Levy Contract":
                case "16-18 Apprenticeship Non-Levy Contract (procured)":
                case "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)":
                    return 17;
                case "19+ Apprenticeship (From May 2017) Levy Contract":
                case "19+ Apprenticeship Non-Levy Contract (procured)":
                case "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)":
                case "19-24 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)":
                    return 21;
                default:
                    throw new ArgumentException("A valid funding line type is required.", nameof(fundingLineType));
            }
        }

        public static int AsPercentage(this string sfaContributionPercentage)
        {
            if (string.IsNullOrWhiteSpace(sfaContributionPercentage) ||
                !sfaContributionPercentage.Contains("%") || !int.TryParse(
                    sfaContributionPercentage.Split('%')[0],
                    out _))
            {
                throw new InvalidCastException("SfaContributionPercentage is not in the format: xx% (e.g. 90%)");
            }

            return
                int.Parse((100 - int.Parse(sfaContributionPercentage.Split('%')[0])).ToString());
        }

    }
}
