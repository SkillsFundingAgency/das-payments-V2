using System;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public static class LearnerExtensions
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
