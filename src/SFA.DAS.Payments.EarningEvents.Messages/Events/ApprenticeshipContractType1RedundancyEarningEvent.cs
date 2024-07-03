using SFA.DAS.Payments.Messages.Common;
using SFA.DAS.Payments.Messages.Common.Events;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class ApprenticeshipContractType1RedundancyEarningEvent : ApprenticeshipContractTypeEarningsEvent, IContractType1EarningEvent, IMonitoredMessage
    {
        public ApprenticeshipContractType1RedundancyEarningEvent()
        {
            SfaContributionPercentage = 1m;
        }
        public string AgreementId { get; set; }
    }
}