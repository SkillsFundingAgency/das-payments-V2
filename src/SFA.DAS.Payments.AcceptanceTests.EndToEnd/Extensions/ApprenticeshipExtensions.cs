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



        public static ApprenticeshipPaymentStatus ToApprenticeshipPaymentStatus(this string status)
        {
            var apprenticeshipStatus = ApprenticeshipPaymentStatus.Inactive;
            switch (status?.ToLower())
            {
                case "active":
                    apprenticeshipStatus = ApprenticeshipPaymentStatus.Active;
                    break;
                case "cancelled":
                    apprenticeshipStatus = ApprenticeshipPaymentStatus.Stopped;
                    break;
                case "paused":
                    apprenticeshipStatus = ApprenticeshipPaymentStatus.Paused;
                    break;
            }

            return apprenticeshipStatus;
        }
    }
}
