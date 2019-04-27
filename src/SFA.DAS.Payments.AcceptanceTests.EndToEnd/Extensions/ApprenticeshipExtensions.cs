using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions
{
    public static class ApprenticeshipExtensions
    {
        public static ApprenticeshipStatus ToApprenticeshipPaymentStatus(this string status)
        {
            var apprenticeshipStatus = ApprenticeshipStatus.Inactive;
            switch (status?.ToLower())
            {
                case "active":
                    apprenticeshipStatus = ApprenticeshipStatus.Active;
                    break;
                case "cancelled":
                    apprenticeshipStatus = ApprenticeshipStatus.Stopped;
                    break;
                case "paused":
                    apprenticeshipStatus = ApprenticeshipStatus.Paused;
                    break;
            }

            return apprenticeshipStatus;
        }
    }
}
