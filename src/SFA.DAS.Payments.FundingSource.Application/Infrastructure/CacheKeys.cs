namespace SFA.DAS.Payments.FundingSource.Application.Infrastructure
{
    public static class CacheKeys
    {
        public static readonly string MonthEndCacheKey = "MonthEndPaymentsProcessed";
        public static readonly string KeyListKey = "keys";
        public static readonly string LevyBalanceKey = "EmployerLevyAccountBalance";
        public const string EmployerPaymentPriorities = "EmployerPaymentPriorities";
    }
}