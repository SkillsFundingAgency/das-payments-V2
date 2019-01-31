using System;
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

        protected override FundingSourcePayment CreatePayment(RequiredCoInvestedPayment requiredPayment)
        {
            var amountToPay = requiredPayment.SfaContributionPercentage * requiredPayment.AmountDue;

            var unallocated = requiredPayment.AmountDue - requiredPayment.AmountFunded;

            amountToPay = Math.Min(amountToPay, unallocated);

            return new SfaCoInvestedPayment
            {
                AmountDue = amountToPay.AsRounded(),
                Type = FundingSourceType.CoInvestedSfa,

            };
        }
    }
}