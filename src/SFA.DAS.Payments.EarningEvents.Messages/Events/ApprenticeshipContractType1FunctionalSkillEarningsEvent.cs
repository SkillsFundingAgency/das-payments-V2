using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class ApprenticeshipContractType1FunctionalSkillEarningsEvent : ApprenticeshipContractTypeFunctionalSkillEarningsEvent
    {
        public ApprenticeshipContractType1FunctionalSkillEarningsEvent()
        {
            ContractType = ContractType.Act1;
        }
    }
}