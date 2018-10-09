using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;

namespace SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner
{
    public class LearnerValidator : ILearnerValidator
    {
        protected static readonly List<ILearnerValidationRule> LearnerRules = new List<ILearnerValidationRule>
        {
            new OverlappingPriceEpisodeValidationRule()
        };

        public List<ValidationRuleResult> Validate(FM36Learner learner)
        {
            return LearnerRules
                .Select(rule => rule.IsValid(learner))
                .ToList();
        }
    }
}