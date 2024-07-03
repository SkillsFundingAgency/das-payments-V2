using SFA.DAS.Payments.Messages.Common;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class ApprenticeshipContractTypeNoneEarningEvent : ApprenticeshipContractTypeEarningsEvent, ILeafLevelMessage
    {
        public override bool IsPayable => false;
    }
}