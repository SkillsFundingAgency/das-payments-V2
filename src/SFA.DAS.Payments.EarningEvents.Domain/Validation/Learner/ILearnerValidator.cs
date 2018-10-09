using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;

namespace SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner
{
    public interface ILearnerValidator
    {
        List<ValidationRuleResult> Validate(FM36Learner learner);
    }
}