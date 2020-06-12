using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.LevyAccountData
{
    public static class ListExtensions
    {
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }
    }
}
