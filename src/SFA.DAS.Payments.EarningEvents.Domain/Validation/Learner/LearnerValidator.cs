using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Core.Validation;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner.Rules;

namespace SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner
{
    public class LearnerValidator : ILearnerValidator
    {
        protected static readonly List<ILearnerValidationRule> LearnerRules = new List<ILearnerValidationRule>
        {
            new OverlappingPriceEpisodeValidationRule()
        };

        public ValidationResult Validate(FM36Learner learner)
        {
            return new ValidationResult( LearnerRules
                .Select(rule => rule.IsValid(learner))
                .ToList());
        }
    }
}