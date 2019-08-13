using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class PayableFunctionalSkillEarningEvent: FunctionalSkillDataLockEvent
    {
        public PayableFunctionalSkillEarningEvent()
        {
            ContractType = ContractType.Act1;
        }
    }
}
