

namespace SFA.DAS.Payments.FundingSource.Domain.Models
{
    public class RequiredPaymentEventValidationResult
    {
        public RequiredPaymentEventValidationRules Rule { get; set; }

        public CoInvestedPayment RequiredCoInvestedPayment { get; set; }
    }
}