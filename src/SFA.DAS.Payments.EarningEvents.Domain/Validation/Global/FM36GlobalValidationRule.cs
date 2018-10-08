using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;

namespace SFA.DAS.Payments.EarningEvents.Domain.Validation.Global
{
    // ReSharper disable once InconsistentNaming
    public class FM36GlobalValidationRule: IFM36GlobalValidationRule
    {
        public ValidationRuleResult IsValid(FM36Global global)
        {
            if (global.UKPRN == 0)
                return ValidationRuleResult.Failed("Invalid ukprn");

            if (string.IsNullOrWhiteSpace(global.Year))
                return ValidationRuleResult.Failed("Empty collection year.");

            return ValidationRuleResult.Ok();
        }
    }
}