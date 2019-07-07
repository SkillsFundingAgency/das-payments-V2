namespace SFA.DAS.Payments.PeriodEnd.Application.Infrastructure
{
    public class JobContextMessageConstants
    {
        public class Tasks
        {
            public static readonly string PeriodEndStart = "PeriodEndStart";
            public static readonly string PeriodEndRun = "PeriodEndRun";
            public static readonly string PeriodEndStop = "PeriodEndStop";
        }

        public class KeyValuePairs
        {
            public static readonly string ReturnPeriod = "ReturnPeriod";
            public static readonly string CollectionYear = "CollectionYear";
        }
    }
}