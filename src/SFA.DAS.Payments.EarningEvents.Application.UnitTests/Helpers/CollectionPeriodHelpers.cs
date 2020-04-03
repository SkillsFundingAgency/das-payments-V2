using System;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests.Helpers
{
    public static class CollectionPeriodHelpers
    {
                
        public static byte GetCollectionPeriodFromDate(this DateTime date)
        {
            byte period;
            var month = date.Month;

            if (month < 8)
            {
                period = (byte) (month + 5);
            }
            else
            {
                period = (byte) (month - 7);
            }
            return period;
        }
    }
}