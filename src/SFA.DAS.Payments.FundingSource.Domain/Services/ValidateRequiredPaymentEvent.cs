using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Collections.Generic;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class ValidateRequiredPaymentEvent : IValidateRequiredPaymentEvent
    {
        public IEnumerable<RequiredPaymentEventValidationResult> Validate(ApprenticeshipContractType2RequiredPaymentEvent message)
        {
            var results = new List<RequiredPaymentEventValidationResult>();

            if (message.SfaContributionPercentage <= 0)
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