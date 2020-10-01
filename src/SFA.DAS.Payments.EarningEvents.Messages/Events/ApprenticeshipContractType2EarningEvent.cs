using System;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class ApprenticeshipContractType2EarningEvent : ApprenticeshipContractTypeEarningsEvent, ILeafLevelMessage
    {
        public ApprenticeshipContractType2EarningEvent()
        {
            SfaContributionPercentage = 1m;
            EventId = Guid.NewGuid();
        }
    }
}