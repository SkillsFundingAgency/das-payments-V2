using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public List<ApprenticeshipContractTypeEarningsEvent> OriginalAndRedundancyEarningEventIfRequired(ApprenticeshipContractTypeEarningsEvent earningEvent, 
            List<byte> redundancyPeriods)
        {
            var splitResults = new List<ApprenticeshipContractTypeEarningsEvent> {earningEvent};
            if (!redundancyPeriods.Any())
                return splitResults;

            var redundancyEarningEvent = redundancyEarningEventFactory.CreateRedundancyContractTypeEarningsEvent(earningEvent);
            splitResults.Add(redundancyEarningEvent);

            earningEvent.OnProgrammeEarnings.ForEach(ope => { ope.Periods= RemoveRedundancyPeriods(ope.Periods, redundancyPeriods); });
            earningEvent.IncentiveEarnings.ForEach(ie => { ie.Periods= RemoveRedundancyPeriods(ie.Periods, redundancyPeriods); }); 

            redundancyEarningEvent.OnProgrammeEarnings.ForEach(ope =>
            {
                ope.Periods=  RemoveNonRedundancyPeriods(ope.Periods, redundancyPeriods);
                SetPeriodsToFullContribution(ope.Periods);
            });
            redundancyEarningEvent.IncentiveEarnings.ForEach(ie =>
            {
                ie.Periods= RemoveNonRedundancyPeriods(ie.Periods, redundancyPeriods);
                SetPeriodsToFullContribution(ie.Periods);
            });

            return splitResults;
        }

        public List<FunctionalSkillEarningsEvent> OriginalAndRedundancyFunctionalSkillEarningEventIfRequired(FunctionalSkillEarningsEvent functionalSkillEarning, List<byte> redundancyPeriods)
        {
            var splitResults = new List<FunctionalSkillEarningsEvent>{ functionalSkillEarning };
            if (!redundancyPeriods.Any())
                return splitResults;

            var redundancyEarningEvent = redundancyEarningEventFactory.CreateRedundancyFunctionalSkillTypeEarningsEvent(functionalSkillEarning);
            splitResults.Add(redundancyEarningEvent);

            foreach (var earning in functionalSkillEarning.Earnings)
            {
               earning.Periods = RemoveRedundancyPeriods(earning.Periods, redundancyPeriods);
            } 
            
            foreach (var earning in redundancyEarningEvent.Earnings)
            {
               earning.Periods = RemoveNonRedundancyPeriods(earning.Periods, redundancyPeriods);
               SetPeriodsToFullContribution(earning.Periods);
            }

            return splitResults;
        }
        
        private static void SetPeriodsToFullContribution(IEnumerable<EarningPeriod> periods)
        {
            foreach (var onProgPeriod in periods)
            {
                onProgPeriod.SfaContributionPercentage = 1m;
            }
        }

        private ReadOnlyCollection<EarningPeriod> RemoveRedundancyPeriods(IEnumerable<EarningPeriod> periods, List<byte> redundancyPeriods)
        {
            var allPeriods = periods.ToList();
            allPeriods.RemoveAll(p => redundancyPeriods.Contains(p.Period));
            return allPeriods.AsReadOnly();
        }

        private ReadOnlyCollection<EarningPeriod> RemoveNonRedundancyPeriods(IEnumerable<EarningPeriod> periods, List<byte> redundancyPeriods)
        {
            var allPeriods = periods.ToList();
            allPeriods.RemoveAll(p => !redundancyPeriods.Contains(p.Period));
            return allPeriods.AsReadOnly();
        }
    }
}