using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class ValidateRequiredPaymentEvent : IValidateRequiredPaymentEvent
    {
        public IEnumerable<RequiredCoInvestedPaymentValidationResult> Validate(RequiredPayment requiredPayment)
        {
            var results = new List<RequiredCoInvestedPaymentValidationResult>();

            if (requiredPayment.SfaContributionPercentage <= 0)
            {
                results.Add(new RequiredCoInvestedPaymentValidationResult
                {
                    Rule = RequiredPaymentEventValidationRules.ZeroSfaContributionPercentage
                });
            }

            return results;
        }
    }
}