using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class ValidateRequiredPaymentEvent : IValidateRequiredPaymentEvent
    {
        public IEnumerable<RequiredPaymentEventValidationResult> Validate(CoInvestedPayment requiredPayment)
        {
            var results = new List<RequiredPaymentEventValidationResult>();

            if (requiredPayment.SfaContributionPercentage <= 0)
            {
                results.Add(new RequiredPaymentEventValidationResult
                {
                    Rule = RequiredPaymentEventValidationRules.ZeroSfaContributionPercentage
                });
            }

            return results;
        }
    }
}