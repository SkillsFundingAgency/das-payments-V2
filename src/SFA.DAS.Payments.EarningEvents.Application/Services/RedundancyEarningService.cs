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


            var redundancyPeriod = GetPeriodFromDate(redundancyDate);  //get period from redundancy date

            //map earning event to correct redundancy type
            var redundancyEarningEvent = redundancyEarningEventFactory.CreateRedundancyContractType(earningEvent);

            //remove redundancy periods from Earning event
            earningEvent.OnProgrammeEarnings.ForEach(ope => { RemoveRedundancyEarningPeriods(ope, redundancyPeriod); });
            earningEvent.IncentiveEarnings.ForEach(ie => { RemoveRedundancyEarningPeriods(ie, redundancyPeriod); }); 

            //set correct values for redundancy periods
            redundancyEarningEvent.OnProgrammeEarnings.ForEach(ope =>
            {
                RemovePreRedundancyEarningPeriods(ope, redundancyPeriod);

                foreach (var onProgPeriod in ope.Periods)
                {
                    if (onProgPeriod.Period >= redundancyPeriod)
                        onProgPeriod.SfaContributionPercentage = 1m;
                }
            });
            redundancyEarningEvent.IncentiveEarnings.ForEach(ie =>
            {
                RemovePreRedundancyEarningPeriods(ie, redundancyPeriod);

                foreach (var incentivePeriod in ie.Periods)
                {
                    if (incentivePeriod.Period >= redundancyPeriod)
                        incentivePeriod.SfaContributionPercentage = 1m;
                }
            });

            splitResults.Add(earningEvent);
            splitResults.Add(redundancyEarningEvent);
            
            return splitResults;
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
            var redundancyPeriod = GetPeriodFromDate(priceEpisodeRedStartDate);  //get period from redundancy date

            //map earning event to correct redundancy type
            var redundancyEarningEvent = redundancyEarningEventFactory.CreateRedundancyFunctionalSkillType(functionalSkillEarning);
            
            //clear functionalSkills period
            foreach (var earning in functionalSkillEarning.Earnings)
            {
                var periods = earning.Periods.ToList();
                periods.RemoveAll(p => p.Period >= redundancyPeriod);
                earning.Periods = periods.AsReadOnly();
            } 
            
            //update redundancy earning period
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