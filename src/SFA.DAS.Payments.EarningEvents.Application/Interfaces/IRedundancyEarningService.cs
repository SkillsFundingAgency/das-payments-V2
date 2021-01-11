using System;
using System.Collections.Generic;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Interfaces
{
    public interface IRedundancyEarningService
    {
        List<ApprenticeshipContractTypeEarningsEvent> SplitContractEarningByRedundancyDate(
            ApprenticeshipContractTypeEarningsEvent earningEvent, List<byte> redundancyPeriods);

        List<FunctionalSkillEarningsEvent> SplitFunctionSkillEarningByRedundancyDate(
            FunctionalSkillEarningsEvent functionalSkillEarning, List<byte> redundancyPeriods);
    }
}