using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    /// <summary>
    /// Earning events for sub aims such as Maths & English
    /// </summary>
    /// <seealso cref="EarningEvent" />
    public class ApprenticeshipContract2TypeFunctionalSkillEarningsEvent : ApprenticeshipContractTypeFunctionalSkillEarningsEvent
    {
        public ApprenticeshipContract2TypeFunctionalSkillEarningsEvent()
        {
            ContractType = ContractType.Act2;
        }
    }
}