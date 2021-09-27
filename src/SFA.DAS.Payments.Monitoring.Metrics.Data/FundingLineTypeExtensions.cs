using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data
{
    public static class FundingLineTypeExtensions
    {
        public static int ToLearnerAgeBanding(this string fundingLineType)
        {
            switch (fundingLineType)
            {
                case "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)":
                case "16-18 Apprenticeship Non-Levy Contract (procured)":
                case "16-18 Apprenticeship (Employer on App Service)":
                    return 16;
                case "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)":
                case "19+ Apprenticeship Non-Levy Contract (procured)":
                case "19+ Apprenticeship (Employer on App Service)":
                    return 19;
                default:
                    return 0;
            }
        }
    }
}
