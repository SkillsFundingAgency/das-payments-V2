using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;

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
            var splitResults = new List<ApprenticeshipContractTypeEarningsEvent>();
          
            var redundancyPeriod = redundancyDate.GetPeriodFromDate(); 

            var redundancyEarningEvent = redundancyEarningEventFactory.CreateRedundancyContractTypeEarningsEvent(earningEvent);

            earningEvent.OnProgrammeEarnings.ForEach(ope => { ope.Periods= RemoveRedundancyPeriods(ope.Periods, redundancyPeriod); });
            earningEvent.IncentiveEarnings.ForEach(ie => { ie.Periods= RemoveRedundancyPeriods(ie.Periods, redundancyPeriod); }); 

            redundancyEarningEvent.OnProgrammeEarnings.ForEach(ope =>
            {
              ope.Periods=  RemovePreRedundancyPeriods(ope.Periods, redundancyPeriod); 
              SetPeriodsToFullContribution(ope.Periods);
            });
            redundancyEarningEvent.IncentiveEarnings.ForEach(ie =>
            {
               ie.Periods= RemovePreRedundancyPeriods(ie.Periods, redundancyPeriod);
                SetPeriodsToFullContribution(ie.Periods);
            });

            splitResults.Add(earningEvent);
            splitResults.Add(redundancyEarningEvent);
            
            return splitResults;
        }

        public List<FunctionalSkillEarningsEvent> SplitFunctionSkillEarningByRedundancyDate(FunctionalSkillEarningsEvent functionalSkillEarning,
            DateTime priceEpisodeRedStartDate)
        {
            var splitResults = new List<FunctionalSkillEarningsEvent>();
            var redundancyPeriod = priceEpisodeRedStartDate.GetPeriodFromDate();

            var redundancyEarningEvent = redundancyEarningEventFactory.CreateRedundancyFunctionalSkillTypeEarningsEvent(functionalSkillEarning);
            
            foreach (var earning in functionalSkillEarning.Earnings)
            {
               earning.Periods = RemoveRedundancyPeriods(earning.Periods, redundancyPeriod);
            } 
            
            foreach (var earning in redundancyEarningEvent.Earnings)
            {
               earning.Periods = RemovePreRedundancyPeriods(earning.Periods, redundancyPeriod);
               SetPeriodsToFullContribution(earning.Periods);
            }

            splitResults.Add(functionalSkillEarning);
            splitResults.Add(redundancyEarningEvent);

            return splitResults;
        }
        
        private static void SetPeriodsToFullContribution(IEnumerable<EarningPeriod> periods)
        {
            foreach (var onProgPeriod in periods)
            {
                onProgPeriod.SfaContributionPercentage = 1m;
            }
        }

        private ReadOnlyCollection<EarningPeriod> RemoveRedundancyPeriods(IEnumerable<EarningPeriod> periods, byte redundancyPeriod)
        {
            var allPeriods = periods.ToList();
            allPeriods.RemoveAll(p => p.Period >= redundancyPeriod);
            return allPeriods.AsReadOnly();
        }

        private ReadOnlyCollection<EarningPeriod> RemovePreRedundancyPeriods(IEnumerable<EarningPeriod> periods, byte redundancyPeriod)
        {
            var allPeriods = periods.ToList();
            allPeriods.RemoveAll(p => p.Period < redundancyPeriod);
            return allPeriods.AsReadOnly();
        }
    }
}