using SFA.DAS.Payments.Messages.Common;
using SFA.DAS.Payments.Messages.Common.Events;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class ApprenticeshipContractType1EarningEvent : ApprenticeshipContractTypeEarningsEvent, IContractType1EarningEvent, IMonitoredMessage
    {
        public string AgreementId { get; set; }
    }
}