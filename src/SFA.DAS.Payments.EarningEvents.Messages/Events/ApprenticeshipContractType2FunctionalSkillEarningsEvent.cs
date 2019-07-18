using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    /// <summary>
    /// Earning events for sub aims such as Maths & English
    /// </summary>
    /// <seealso cref="EarningEvent" />
    public class ApprenticeshipContractType2FunctionalSkillEarningsEvent : ApprenticeshipContractTypeFunctionalSkillEarningsEvent
    {
        public ApprenticeshipContractType2FunctionalSkillEarningsEvent()
        {
            ContractType = ContractType.Act2;
        }
    }
}