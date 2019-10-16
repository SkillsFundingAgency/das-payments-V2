﻿using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Abstract;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FastMember;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner.Rules;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public static class MappingExtensions
    {
        private static readonly TypeAccessor PeriodAccessor = TypeAccessor.Create(typeof(PeriodisedAttribute));
        private static readonly TypeAccessor PeriodTextAccessor = TypeAccessor.Create(typeof(LearningDeliveryPeriodisedTextValues));

        public static decimal? GetPeriodValue(this PeriodisedAttribute periodisedValues, int period)
        {
            return (decimal?)PeriodAccessor[periodisedValues, "Period" + period];
        }

        public static string GetPeriodTextValue(this LearningDeliveryPeriodisedTextValues periodisedValues, int period)
        {
            return PeriodTextAccessor[periodisedValues, "Period" + period].ToString();
        }

        public static ContractType[] GetContractTypesForLearningDeliveries(this List<LearningDelivery> learningDeliveries)
        {
            var periodisedTextValues = learningDeliveries.Where(x => x.LearningDeliveryPeriodisedTextValues != null)
                .SelectMany(x => x.LearningDeliveryPeriodisedTextValues.Where(l => l.AttributeName == "LearnDelContType"))
                .ToList();

            if (!periodisedTextValues.Any())
            {
                return new ContractType[0];
            }

            const byte earningPeriods = 12;

            var contractTypes = new ContractType[earningPeriods];

            for (byte i = 1; i <= earningPeriods; i++)
            {
                var periodValues = periodisedTextValues.Select(p => p.GetPeriodTextValue(i)).ToArray();
                var periodValue = GetContractType(periodValues[0]);

                contractTypes[i - 1] = periodValue;
            }

            return contractTypes;
        }
        
        public static ContractType GetContractType(string priceEpisodeContractType)
        {
            switch (priceEpisodeContractType)
            {
                case ContractTypeValidationRule.Act1:
                case ContractTypeValidationRule.ContractForServicesWithEmployer:
                    return ContractType.Act1;
                default:
                    return ContractType.Act2;
            }
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
                Amount = periodValue?.AsRounded() ?? 0,
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
    }
}