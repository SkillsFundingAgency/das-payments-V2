namespace SFA.DAS.Payments.EarningEvents.Domain.Validation
{
    public class ValidationRuleResult
    {
        public static ValidationRuleResult Ok() => new ValidationRuleResult { IsValid = true };
        public static ValidationRuleResult Failed(string failureReason) => new ValidationRuleResult { IsValid = false, FailureReason = failureReason };

        public bool IsValid { get; set; }
        public string FailureReason { get; set; }
    }
}