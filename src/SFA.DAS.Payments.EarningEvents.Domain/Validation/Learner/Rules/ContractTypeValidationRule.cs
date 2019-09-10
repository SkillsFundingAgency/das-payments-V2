using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Core.Validation;

namespace SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner.Rules
{
    public class ContractTypeValidationRule : ILearnerValidationRule
    {
        public const string Act1 = "Levy Contract";
        public const string Act2 = "Non-Levy Contract";
        public const string ContractForServicesWithEmployer = "Contract for services with the employer";
        public const string ContractForServicesWithSfa = "Contract for services with the ESFA";

        private static readonly string[] ValidContractTypes = {Act1, Act2, ContractForServicesWithEmployer, ContractForServicesWithSfa};

        public ValidationRuleResult IsValid(FM36Learner learner)
        {
            foreach (var priceEpisode in learner.PriceEpisodes)
            {
                if (priceEpisode.PriceEpisodeValues == null)
                    continue;

                if (!ValidContractTypes.Contains(priceEpisode.PriceEpisodeValues.PriceEpisodeContractType))
                    return ValidationRuleResult.Failure($"Unknown contract type: '{priceEpisode.PriceEpisodeValues.PriceEpisodeContractType}'.");
            }

            return  ValidationRuleResult.Ok();
        }
    }
}