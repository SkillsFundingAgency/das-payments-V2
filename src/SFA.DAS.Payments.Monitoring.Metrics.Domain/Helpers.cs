namespace SFA.DAS.Payments.Monitoring.Metrics.Domain
{
    public class Helpers
    {
        public static decimal GetPercentage(decimal amount, decimal total) => amount == total ? 100 : total > 0 ? (amount / total) * 100 : 0;
    }
}