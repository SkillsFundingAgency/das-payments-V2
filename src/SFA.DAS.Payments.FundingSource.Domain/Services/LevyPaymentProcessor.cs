using System;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class LevyPaymentProcessor : ILevyPaymentProcessor
    {
        public FundingSourcePayment Process(RequiredPayment requiredPayment)
        {
            var requiredLevyPayment = (RequiredLevyPayment)requiredPayment;

            if (requiredLevyPayment.LevyBalance == 0)
                return null;

            var amountDue = (1 - requiredPayment.SfaContributionPercentage) * requiredPayment.AmountDue;

            if (amountDue == 0)
                return null;

            var amountToPay = Math.Min(amountDue, requiredLevyPayment.LevyBalance);

            requiredLevyPayment.LevyBalance -= amountToPay;
            requiredLevyPayment.AmountFunded += amountToPay;

            return new LevyPayment
            {
                AmountDue = amountToPay,
                Type = FundingSourceType.Levy  
            };

        }
    }
}