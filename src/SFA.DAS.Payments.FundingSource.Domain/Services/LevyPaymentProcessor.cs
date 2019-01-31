using System;
using SFA.DAS.Payments.Core;
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

            var unallocated = requiredLevyPayment.AmountDue - requiredPayment.AmountFunded;

            if (unallocated == 0)
                return null;

            amountDue = Math.Min(amountDue, unallocated);

            amountDue = Math.Min(amountDue, requiredLevyPayment.LevyBalance).AsRounded();

            requiredLevyPayment.LevyBalance -= amountDue;
            requiredLevyPayment.AmountFunded += amountDue;

            return new LevyPayment
            {
                AmountDue = amountDue,
                Type = FundingSourceType.Levy  
            };

        }
    }
}