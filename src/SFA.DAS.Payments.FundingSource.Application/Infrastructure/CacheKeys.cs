namespace SFA.DAS.Payments.FundingSource.Application.Infrastructure
{
    public static class CacheKeys
    {
        public const string MonthEndCacheKey = "MonthEndPaymentsProcessed";
        public const string KeyListKey = "keys";
        public const string LevyBalanceKey = "EmployerLevyAccountBalance";
        public const string EmployerPaymentPriorities = "EmployerPaymentPriorities";
        public const string RefundPaymentsKeyListKey = "RefundPaymentsKeys";
        public const string SenderTransferKeyListKey = "SenderTransferKeys";
        public const string RequiredPaymentKeyListKey = "RequiredPaymentKeys";
    }
}