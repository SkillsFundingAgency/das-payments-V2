using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FastMember;
using SFA.DAS.Payments.Model.Core;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public static class MappingExtensions
    {
        private static readonly TypeAccessor PeriodAccessor = TypeAccessor.Create(typeof(PriceEpisodePeriodisedValues));

        public static decimal? GetPeriodValue(this PriceEpisodePeriodisedValues periodisedValues, int period)
        {
            return (decimal?) PeriodAccessor[periodisedValues, "Period" + period];
        }

        public static bool IsMainAim(this LearningDelivery learningDelivery)
        {
            return learningDelivery.LearningDeliveryValues.LearnAimRef == "ZPROG001";
        }

        public static void AddPeriodValue(this List<EarningPeriod> earningPeriods, decimal? periodValue, byte period, string priceEpisodeIdentifier, decimal? sfaContributionPercentage = null)
        {
            earningPeriods.Add(new EarningPeriod
            {
                Period = period,
                PriceEpisodeIdentifier = priceEpisodeIdentifier,
                Amount = periodValue ?? 0,
                SfaContributionPercentage = sfaContributionPercentage,
            });
        }

        public static List<EarningPeriod> CreateIncentiveEarningPeriods(this PriceEpisodePeriodisedValues values, string priceEpisodeIdentifier)
        {
            var result = new List<EarningPeriod>();
            result.AddPeriodValue(values.Period1, 1, priceEpisodeIdentifier, 1);
            result.AddPeriodValue(values.Period2, 2, priceEpisodeIdentifier, 1);
            result.AddPeriodValue(values.Period3, 3, priceEpisodeIdentifier, 1);
            result.AddPeriodValue(values.Period4, 4, priceEpisodeIdentifier, 1);
            result.AddPeriodValue(values.Period5, 5, priceEpisodeIdentifier, 1);
            result.AddPeriodValue(values.Period6, 6, priceEpisodeIdentifier, 1);
            result.AddPeriodValue(values.Period7, 7, priceEpisodeIdentifier, 1);
            result.AddPeriodValue(values.Period8, 8, priceEpisodeIdentifier, 1);
            result.AddPeriodValue(values.Period9, 9, priceEpisodeIdentifier, 1);
            result.AddPeriodValue(values.Period10, 10, priceEpisodeIdentifier, 1);
            result.AddPeriodValue(values.Period11, 11, priceEpisodeIdentifier, 1);
            result.AddPeriodValue(values.Period12, 12, priceEpisodeIdentifier, 1);
            return result;
        }

        public static PriceEpisode GetLatestOnProgPriceEpisode(this List<PriceEpisode> priceEpisodes)
        {
            return priceEpisodes
                .Where(IsOnProgPriceEpisode)
                .OrderByDescending(priceEpisode => priceEpisode.PriceEpisodeValues?.EpisodeStartDate)
                .FirstOrDefault();
        }

        private static readonly HashSet<string> OnProgAttributeNames = new HashSet<string>(new[]
        {
            "PriceEpisodeBalancePayment", "PriceEpisodeCompletionPayment", "PriceEpisodeOnProgPayment"
        });

        private static bool IsOnProgPriceEpisode(PriceEpisode priceEpisode)
        {
            if (priceEpisode.PriceEpisodeValues?.EpisodeStartDate == null)
                return false;

            if (priceEpisode.PriceEpisodePeriodisedValues == null)
                return false;

            if (priceEpisode.PriceEpisodePeriodisedValues == null)
                return false;

            return priceEpisode.PriceEpisodePeriodisedValues.Any(p => OnProgAttributeNames.Contains(p.AttributeName));
        }
    }
}