using System;
using DCT.TestDataGenerator;

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

        public static int? AsPercentage(this string sfaContributionPercentage)
        {
            if (string.IsNullOrWhiteSpace(sfaContributionPercentage) ||
                !sfaContributionPercentage.Contains("%") || !int.TryParse(
                    sfaContributionPercentage.Split('%')[0],
                    out _))
            {
                return null;
            }

            return
                int.Parse(sfaContributionPercentage.Split('%')[0]);
        }

        public static int ToEmpStatCode(this string employmentStatus)
        {
            switch (employmentStatus)
            {
                case "in paid employment":
                    return (int) EmploymentStatus.PaidEmployment;
                case "not in paid employment":
                    return (int) EmploymentStatus.LookingForWork;
                default:
                    throw new ArgumentException("A valid employment status is required.", nameof(employmentStatus));
            }
        }

        public static int ToEmployerAccountId(this string employer)
        {
            switch (employer)
            {
                case "employer 2":
                    return 913703206;
                default:
                    return 154549452;
            }
        }
    }
}