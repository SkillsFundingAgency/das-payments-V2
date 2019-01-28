using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class ApprenticeshipContractType1EarningEvent : ApprenticeshipContractTypeEarningsEvent, IContractType1EarningEvent
    {
        public string AgreementId { get; set; }
    }
}