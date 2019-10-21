using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class PayableFunctionalSkillEarningEvent: FunctionalSkillDataLockEvent, IMonitoredMessage, ILeafLevelMessage
    {
        public PayableFunctionalSkillEarningEvent()
        {
            ContractType = ContractType.Act1;
        }
    }
}
