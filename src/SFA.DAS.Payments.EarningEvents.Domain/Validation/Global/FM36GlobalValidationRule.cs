using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Core.Validation;

namespace SFA.DAS.Payments.EarningEvents.Domain.Validation.Global
{
    // ReSharper disable once InconsistentNaming
    public class FM36GlobalValidationRule: IFM36GlobalValidationRule
    {
        public ValidationRuleResult IsValid(FM36Global global)
        {
            if (global.UKPRN == 0)
                return ValidationRuleResult.Failure("Invalid ukprn");

            if (string.IsNullOrWhiteSpace(global.Year))
                return ValidationRuleResult.Failure("Empty collection year.");

            return ValidationRuleResult.Ok();
        }
    }
}