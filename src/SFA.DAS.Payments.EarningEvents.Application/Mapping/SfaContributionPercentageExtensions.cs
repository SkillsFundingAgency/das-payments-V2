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
            var value = priceEpisode.PriceEpisodePeriodisedValues
                .Where(x => x.AttributeName == "PriceEpisodeSFAContribPct")
                .Select(p => (decimal?)PeriodAccessor[p, "Period" + period])
                .SingleOrDefault();

            if (value == null)
            {
                value = priceEpisode.PriceEpisodeValues.PriceEpisodeSFAContribPct;
            }

            return value;
        }
    }
}