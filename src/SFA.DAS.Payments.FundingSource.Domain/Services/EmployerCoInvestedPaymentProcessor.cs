using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class EmployerCoInvestedPaymentProcessor : CoInvestedPaymentProcessorBase, IEmployerCoInvestedPaymentProcessor
    {

        public EmployerCoInvestedPaymentProcessor(IValidateRequiredPaymentEvent validateRequiredPaymentEvent)
            : base(validateRequiredPaymentEvent)
        {
        }

        protected override FundingSourcePayment CreatePayment(RequiredPayment requiredPayment)
        {
            var amountToPay = (1 - requiredPayment.SfaContributionPercentage) * requiredPayment.AmountDue;

            return new EmployerCoInvestedPayment
            {
                AmountDue = amountToPay.AsRounded(),
                Type = FundingSourceType.CoInvestedEmployer,
                FundingPlatformType = requiredPayment.FundingPlatformType
            };
        }
    }
}