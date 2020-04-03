using System;
using System.Collections.Generic;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Interfaces
{
    public interface IRedundancyEarningService
    {
        List<ApprenticeshipContractTypeEarningsEvent> SplitContractEarningByRedundancyDate(
            ApprenticeshipContractTypeEarningsEvent earningEvent, DateTime redundancyDate);

        List<ApprenticeshipContractTypeEarningsEvent> SplitContractEarningsByRedundancyDate(
            List<ApprenticeshipContractTypeEarningsEvent> earningEvents, DateTime redundancyDate);

        List<FunctionalSkillEarningsEvent> SplitFunctionSkillEarningByRedundancyDate(FunctionalSkillEarningsEvent functionalSkillEarning, DateTime priceEpisodeRedStartDate);
    }
}