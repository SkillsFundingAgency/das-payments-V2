using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Application.Services
{
    public class RedundancyEarningService : IRedundancyEarningService
    {
        private readonly IRedundancyEarningEventFactory redundancyEarningEventFactory;

        public RedundancyEarningService(IRedundancyEarningEventFactory redundancyEarningEventFactory)
        {
            this.redundancyEarningEventFactory = redundancyEarningEventFactory ?? throw new ArgumentNullException(nameof(redundancyEarningEventFactory));
        }

        public List<ApprenticeshipContractTypeEarningsEvent> SplitContractEarningByRedundancyDate(ApprenticeshipContractTypeEarningsEvent earningEvent, DateTime redundancyDate)
        {
            List<ApprenticeshipContractTypeEarningsEvent> splitResults = new List<ApprenticeshipContractTypeEarningsEvent>();

            var redundancyPeriod = GetPeriodFromDate(redundancyDate); 

            var redundancyEarningEvent = redundancyEarningEventFactory.CreateRedundancyContractType(earningEvent);

            earningEvent.OnProgrammeEarnings.ForEach(ope => { RemoveRedundancyEarningPeriods(ope, redundancyPeriod); });
            earningEvent.IncentiveEarnings.ForEach(ie => { RemoveRedundancyEarningPeriods(ie, redundancyPeriod); }); 

            redundancyEarningEvent.OnProgrammeEarnings.ForEach(ope =>
            {
                RemovePreRedundancyEarningPeriods(ope, redundancyPeriod);
                SetRedundancyPeriodsToFullContribution(ope);
            });
            redundancyEarningEvent.IncentiveEarnings.ForEach(ie =>
            {
                RemovePreRedundancyEarningPeriods(ie, redundancyPeriod);
                SetRedundancyPeriodsToFullContribution(ie);
            });

            splitResults.Add(earningEvent);
            splitResults.Add(redundancyEarningEvent);
            
            return splitResults;
        }

        private static void SetRedundancyPeriodsToFullContribution(Earning ope)
        {
            foreach (var onProgPeriod in ope.Periods)
            {
                onProgPeriod.SfaContributionPercentage = 1m;
            }
        }

        private void RemoveRedundancyEarningPeriods(Earning earning, byte redundancyPeriod)
        {
            var periods = earning.Periods.ToList();
            periods.RemoveAll(p => p.Period >= redundancyPeriod);
            earning.Periods = periods.AsReadOnly();
        }

        private void RemovePreRedundancyEarningPeriods(Earning earning, byte redundancyPeriod)
        {
            var periods = earning.Periods.ToList();
            periods.RemoveAll(p => p.Period < redundancyPeriod);
            earning.Periods = periods.AsReadOnly();
        }

        public List<FunctionalSkillEarningsEvent> SplitFunctionSkillEarningByRedundancyDate(FunctionalSkillEarningsEvent functionalSkillEarning,
            DateTime priceEpisodeRedStartDate)
        {
            List<FunctionalSkillEarningsEvent> splitResults = new List<FunctionalSkillEarningsEvent>();
            var redundancyPeriod = GetPeriodFromDate(priceEpisodeRedStartDate);

            var redundancyEarningEvent = redundancyEarningEventFactory.CreateRedundancyFunctionalSkillType(functionalSkillEarning);
            
            foreach (var earning in functionalSkillEarning.Earnings)
            {
                var periods = earning.Periods.ToList();
                periods.RemoveAll(p => p.Period >= redundancyPeriod);
                earning.Periods = periods.AsReadOnly();
            } 
            
            foreach (var earning in redundancyEarningEvent.Earnings)
            {
                var periods = earning.Periods.ToList();
                periods.RemoveAll(p => p.Period < redundancyPeriod);
                earning.Periods = periods.AsReadOnly();

                foreach (var period in earning.Periods)
                {
                    if (period.Period >= redundancyPeriod)
                    {
                        period.SfaContributionPercentage = 1m;
                    }
                }
            }

            splitResults.Add(functionalSkillEarning);
            splitResults.Add(redundancyEarningEvent);

            return splitResults;
        }

        private byte GetPeriodFromDate(DateTime date)
        {
            byte period;
            var month = date.Month;

            if (month < 8)
            {
                period = (byte) (month + 5);
            }
            else
            {
                period = (byte) (month - 7);
            }
            return period;
        }
    }
}