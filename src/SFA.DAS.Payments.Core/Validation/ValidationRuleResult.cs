namespace SFA.DAS.Payments.Core.Validation
{
    public class ValidationRuleResult
    {
        public static ValidationRuleResult Ok() => new ValidationRuleResult { Failed = false };
        public static ValidationRuleResult Failure(string failureReason) => new ValidationRuleResult { Failed = true, FailureReason = failureReason };

        public bool Failed { get; private set; }
        public string FailureReason { get; private set; }
    }
}