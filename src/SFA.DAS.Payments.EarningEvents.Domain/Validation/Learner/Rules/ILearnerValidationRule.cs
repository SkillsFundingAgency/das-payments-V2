using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Core.Validation;

namespace SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner.Rules
{
    public interface ILearnerValidationRule
    {
        ValidationRuleResult IsValid(FM36Learner learner);
    }
}