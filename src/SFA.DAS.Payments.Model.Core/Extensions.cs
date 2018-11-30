using System.Linq;

namespace SFA.DAS.Payments.Model.Core
{
    public static class Extensions
    {
        public static string GetCollectionYear(this CalendarPeriod calendarPeriod)
        {
            return calendarPeriod.Name.Split('-').FirstOrDefault();
        }
    }
}