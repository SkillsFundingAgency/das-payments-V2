using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class EmployerCoInvestedPaymentProcessor : CoInvestedPaymentProcessor
    {

        public EmployerCoInvestedPaymentProcessor(IValidateRequiredPaymentEvent validateRequiredPaymentEvent)
            : base(validateRequiredPaymentEvent)
        {
        }

        protected override Payment CreatePayment(CoInvestedPayment message)
        {
            var amountToPay = (1 - message.SfaContributionPercentage) * message.AmountDue;
            return new Payment
            {
                AmountDue = amountToPay,
                Type = Enum.FundingSourceType.CoInvestedEmployer
            };
        }
    }
}