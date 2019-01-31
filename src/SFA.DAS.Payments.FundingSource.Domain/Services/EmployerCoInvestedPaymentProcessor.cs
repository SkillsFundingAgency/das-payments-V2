using System;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class EmployerCoInvestedPaymentProcessor : CoInvestedPaymentProcessor
    {

        public EmployerCoInvestedPaymentProcessor(IValidateRequiredPaymentEvent validateRequiredPaymentEvent)
            : base(validateRequiredPaymentEvent)
        {
        }

        protected override FundingSourcePayment CreatePayment(RequiredCoInvestedPayment requiredPayment)
        {
            var amountToPay = (1 - requiredPayment.SfaContributionPercentage) * requiredPayment.AmountDue;

            var unallocated = requiredPayment.AmountDue - requiredPayment.AmountFunded;

            amountToPay = amountToPay >= 0 ? Math.Min(amountToPay, unallocated) : Math.Max(amountToPay, unallocated);

            return new EmployerCoInvestedPayment
            {
                AmountDue = amountToPay.AsRounded(),
                Type = FundingSourceType.CoInvestedEmployer
            };
        }
    }
}