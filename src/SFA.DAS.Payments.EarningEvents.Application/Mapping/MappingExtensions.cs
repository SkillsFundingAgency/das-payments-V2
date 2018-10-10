using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public static class MappingExtensions
    {
        public static void AddPeriodValue(this List<EarningPeriod> earningPeriods, decimal? periodValue, byte period,
            string priceEpisodeIdentifier)
        {
            if (periodValue.HasValue && periodValue > 0)
                earningPeriods.Add(new EarningPeriod { Period = period, PriceEpisodeIdentifier = priceEpisodeIdentifier, Amount = periodValue.Value });
        }

        public static List<EarningPeriod> CreateEarningPeriods(this PriceEpisodePeriodisedValues values, string priceEpisodeIdentifier)
        {
            var result = new List<EarningPeriod>();
            result.AddPeriodValue(values.Period1, 1, priceEpisodeIdentifier);
            result.AddPeriodValue(values.Period2, 2, priceEpisodeIdentifier);
            result.AddPeriodValue(values.Period3, 3, priceEpisodeIdentifier);
            result.AddPeriodValue(values.Period4, 4, priceEpisodeIdentifier);
            result.AddPeriodValue(values.Period5, 5, priceEpisodeIdentifier);
            result.AddPeriodValue(values.Period6, 6, priceEpisodeIdentifier);
            result.AddPeriodValue(values.Period7, 7, priceEpisodeIdentifier);
            result.AddPeriodValue(values.Period8, 8, priceEpisodeIdentifier);
            result.AddPeriodValue(values.Period9, 9, priceEpisodeIdentifier);
            result.AddPeriodValue(values.Period10, 10, priceEpisodeIdentifier);
            result.AddPeriodValue(values.Period11, 11, priceEpisodeIdentifier);
            result.AddPeriodValue(values.Period12, 12, priceEpisodeIdentifier);
            return result;
        }
    }
}