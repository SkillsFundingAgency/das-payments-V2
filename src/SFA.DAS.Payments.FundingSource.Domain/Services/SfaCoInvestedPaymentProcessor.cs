using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class SfaCoInvestedPaymentProcessor : CoInvestedPaymentProcessorBase, ISfaCoInvestedPaymentProcessor
    {
        public SfaCoInvestedPaymentProcessor(IValidateRequiredPaymentEvent validateRequiredPaymentEvent)
            : base(validateRequiredPaymentEvent)
        {
        }

        protected override FundingSourcePayment CreatePayment(RequiredPayment requiredPayment)
        {
            var amountToPay = requiredPayment.SfaContributionPercentage * requiredPayment.AmountDue;

            return new SfaCoInvestedPayment
            {
                AmountDue = amountToPay.AsRounded(),
                Type = FundingSourceType.CoInvestedSfa,
                FundingPlatformType = requiredPayment.FundingPlatformType
            };
        }
    }
}