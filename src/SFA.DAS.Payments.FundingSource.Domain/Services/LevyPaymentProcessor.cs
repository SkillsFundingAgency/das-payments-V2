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

            if (requiredLevyPayment.LevyBalance == 0 && requiredPayment.AmountDue > 0 || requiredPayment.AmountDue == 0)
                return null;

            var amountDue = requiredPayment.AmountDue;

            var unallocated = requiredLevyPayment.AmountDue - requiredPayment.AmountFunded;

            if (unallocated == 0)
                return null;

            amountDue = amountDue >= 0 ? Math.Min(amountDue, unallocated) : Math.Max(amountDue, unallocated);

            if (amountDue > 0)
                amountDue = Math.Min(amountDue, requiredLevyPayment.LevyBalance);

            amountDue = amountDue.AsRounded();

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