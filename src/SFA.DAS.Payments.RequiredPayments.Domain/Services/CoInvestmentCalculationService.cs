using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public interface ICoInvestmentCalculationService
    {
        bool IsEligibleForRecalculation(PayableEarningEvent payableEarningEvent);
        EarningType GetEarningType(int type);

        IReadOnlyCollection<(EarningPeriod period, int type)> ProcessPeriodsForRecalculation(
            IReadOnlyCollection<(EarningPeriod period, int type)> periods);
    }

    public class CoInvestmentCalculationService : ICoInvestmentCalculationService
    {
        private const int AgeThreshold = 22;
        public static readonly DateTime RecalculationStartDate = new(2024, 4, 1);

        public bool IsEligibleForRecalculation(PayableEarningEvent payableEarningEvent)
        {
            if (payableEarningEvent.StartDate >= RecalculationStartDate)
            {
                if (payableEarningEvent.AgeAtStartOfLearning is not null &&
                    payableEarningEvent.AgeAtStartOfLearning < AgeThreshold)
                {
                    return true;
                }
            }

            return false;
        }

        public IReadOnlyCollection<(EarningPeriod period, int type)> ProcessPeriodsForRecalculation(IReadOnlyCollection<(EarningPeriod period, int type)> periods)
        {
            foreach (var earningPeriod in periods)
            {
                //Is data matched?
                if (earningPeriod.period.ApprenticeshipId is null or 0) continue;

                if (earningPeriod.period.DataLockFailures is not null && earningPeriod.period.DataLockFailures.Any()) continue;

                if (GetEarningType(earningPeriod.type) == EarningType.Levy)
                {
                    earningPeriod.period.SfaContributionPercentage = new decimal(1.0);
                }
            }
            return periods;
        }

        public EarningType GetEarningType(int type)
        {
            if (Enum.IsDefined(typeof(OnProgrammeEarningType), type))
            {
                return EarningType.Levy;
            }

            return EarningType.Incentive;
        }
    }
}