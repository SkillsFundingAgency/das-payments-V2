using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class FunctionalSkillEarningFailedDataLockMatching: FunctionalSkillDataLockEvent
    {
        public FunctionalSkillEarningFailedDataLockMatching()
        {
            ContractType = ContractType.Act1;
        }
    }
}
