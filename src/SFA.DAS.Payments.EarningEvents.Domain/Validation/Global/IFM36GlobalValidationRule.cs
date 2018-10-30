using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Core.Validation;

namespace SFA.DAS.Payments.EarningEvents.Domain.Validation.Global
{
    // ReSharper disable once InconsistentNaming
    public interface IFM36GlobalValidationRule
    {
        ValidationRuleResult IsValid(FM36Global global);
    }
}