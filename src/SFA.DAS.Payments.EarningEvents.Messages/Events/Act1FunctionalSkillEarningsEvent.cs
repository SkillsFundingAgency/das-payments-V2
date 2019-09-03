using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class Act1FunctionalSkillEarningsEvent : FunctionalSkillEarningsEvent
    {
        public Act1FunctionalSkillEarningsEvent()
        {
            ContractType = ContractType.Act1;
        }
    }
}