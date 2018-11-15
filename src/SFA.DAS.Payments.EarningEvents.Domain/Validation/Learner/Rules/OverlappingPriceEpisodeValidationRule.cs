using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Core.Validation;

namespace SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner.Rules
{
    public class OverlappingPriceEpisodeValidationRule: ILearnerValidationRule
    {
        public ValidationRuleResult IsValid(FM36Learner learner)
        {
            foreach (var priceEpisode in learner.PriceEpisodes)
            {
                var overlappingPriceEpisode = learner.PriceEpisodes
                    .Where(pe => pe != priceEpisode)
                    .FirstOrDefault(pe =>
                        (priceEpisode.PriceEpisodeValues.PriceEpisodeActualEndDate ?? priceEpisode.PriceEpisodeValues.PriceEpisodePlannedEndDate) > pe.PriceEpisodeValues?.EpisodeStartDate &&
                        priceEpisode.PriceEpisodeValues.EpisodeStartDate < (pe.PriceEpisodeValues.PriceEpisodeActualEndDate ?? pe.PriceEpisodeValues?.PriceEpisodePlannedEndDate));
                if (overlappingPriceEpisode != null)
                    return ValidationRuleResult.Failure($"Found overlapping price episodes.  Price Episode {priceEpisode.PriceEpisodeIdentifier} overlapped with price episode {overlappingPriceEpisode.PriceEpisodeIdentifier}.");
            }
            return  ValidationRuleResult.Ok();
        }
    }
}