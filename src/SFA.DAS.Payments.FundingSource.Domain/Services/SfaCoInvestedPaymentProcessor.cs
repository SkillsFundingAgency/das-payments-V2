using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class SfaCoInvestedPaymentProcessor : CoInvestedPaymentProcessor
    {

        public SfaCoInvestedPaymentProcessor(IValidateRequiredPaymentEvent validateRequiredPaymentEvent)
            : base(validateRequiredPaymentEvent)
        {
        }

        protected override Payment CreatePayment(CoInvestedPayment message)
        {
            var amountToPay = message.SfaContributionPercentage * message.AmountDue;

            return new Payment
            {
                AmountDue = amountToPay,
                 Type = Enum.FundingSourceType.CoInvestedSfa
            };
        }
    }
}