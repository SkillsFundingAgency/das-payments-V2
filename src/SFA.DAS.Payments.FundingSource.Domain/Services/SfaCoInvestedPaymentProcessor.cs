using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class SfaCoInvestedPaymentProcessor : CoInvestedPaymentProcessor
    {

        public SfaCoInvestedPaymentProcessor(IValidateRequiredPaymentEvent validateRequiredPaymentEvent)
            : base(validateRequiredPaymentEvent)
        {
        }

        protected override FundingSourcePayment CreatePayment(RequiredCoInvestedPayment message)
        {
            var amountToPay = message.SfaContributionPercentage * message.AmountDue;

            return new SfaCoInvestedPayment
            {
                AmountDue = amountToPay.AsRounded(),
                Type = FundingSourceType.CoInvestedSfa,

            };
        }
    }
}