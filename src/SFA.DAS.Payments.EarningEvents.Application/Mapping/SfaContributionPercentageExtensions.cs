using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FastMember;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public static class SfaContributionPercentageExtensions
    {
        private static readonly TypeAccessor PeriodAccessor = TypeAccessor.Create(typeof(PriceEpisodePeriodisedValues));

        public static decimal? CalculateSfaContributionPercentage(this List<PriceEpisode> source, int period,
            string priceEpisodeIdentifier)
        {
            if (priceEpisodeIdentifier == null)
            {
                return null;
            }
            var priceEpisode = source.Single(x => x.PriceEpisodeIdentifier == priceEpisodeIdentifier);

            var values =
                priceEpisode.PriceEpisodePeriodisedValues
                    .SingleOrDefault(x => x.AttributeName == "PriceEpisodeESFAContribPct");

            return values == null ? null : (decimal?) PeriodAccessor[values, "Period" + period];
        }
    }
}