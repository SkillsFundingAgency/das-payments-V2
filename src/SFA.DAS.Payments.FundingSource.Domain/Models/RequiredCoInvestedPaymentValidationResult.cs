

namespace SFA.DAS.Payments.FundingSource.Domain.Models
{
    public class RequiredCoInvestedPaymentValidationResult
    {
        public RequiredPaymentEventValidationRules Rule { get; set; }

        public RequiredPayment RequiredCoInvestedPayment { get; set; }
    }
}