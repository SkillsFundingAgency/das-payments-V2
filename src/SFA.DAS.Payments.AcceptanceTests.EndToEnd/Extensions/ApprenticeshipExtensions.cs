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
                case "stopped":
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
