using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class FunctionalSkillEarningFailedDataLockMatching: FunctionalSkillDataLockEvent, IMonitoredMessage
    {
        public FunctionalSkillEarningFailedDataLockMatching()
        {
            ContractType = ContractType.Act1;
        }
    }
}
