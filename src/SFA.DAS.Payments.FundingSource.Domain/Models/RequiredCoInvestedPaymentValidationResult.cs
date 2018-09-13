

namespace SFA.DAS.Payments.FundingSource.Domain.Models
{
    public class RequiredCoInvestedPaymentValidationResult
    {
        public RequiredPaymentEventValidationRules Rule { get; set; }

        public RequiredCoInvestedPayment RequiredCoInvestedPayment { get; set; }
    }
}