namespace SFA.DAS.Payments.JobContextMessageHandling.Infrastructure
{
    public class JobContextMessageConstants
    {
        public class Tasks
        {
            public static readonly string PeriodEndStart = "PeriodEndStart";
            public static readonly string PeriodEndRun = "PeriodEndRun";
            public static readonly string PeriodEndStop = "PeriodEndStop";
            public static readonly string ProcessPeriodEndSubmission = "ProcessPeriodEnd";
            public static readonly string ProcessSubmission = "Process";
        }

        public class KeyValuePairs
        {
            public static readonly string ReturnPeriod = "ReturnPeriod";
            public static readonly string CollectionYear = "CollectionYear";
            public static readonly string Ukprn = "UkPrn";
            public static readonly string Container = "Container";
            public static readonly string FundingFm36Output = "FundingFm36Output";
            public static readonly string FundingFm36OutputPeriodEnd = "FundingFm36OutputPeriodEnd";
            public static readonly string Username = "Username";
            public static readonly string Filename = "Filename";
        }
    }
}