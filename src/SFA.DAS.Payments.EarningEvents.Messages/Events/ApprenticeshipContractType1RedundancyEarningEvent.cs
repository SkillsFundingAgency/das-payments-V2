using System;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class ApprenticeshipContractType1RedundancyEarningEvent : ApprenticeshipContractTypeEarningsEvent, IContractType1EarningEvent, IMonitoredMessage
    {
        public ApprenticeshipContractType1RedundancyEarningEvent()
        {
            SfaContributionPercentage = 1m;
            EventId = Guid.NewGuid();
        }
        public string AgreementId { get; set; }
    }
}