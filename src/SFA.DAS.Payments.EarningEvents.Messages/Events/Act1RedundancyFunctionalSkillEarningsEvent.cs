using System;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class Act1RedundancyFunctionalSkillEarningsEvent :FunctionalSkillEarningsEvent
    {
        public Act1RedundancyFunctionalSkillEarningsEvent()
        {
            ContractType = ContractType.Act1;
            EventId = Guid.NewGuid();
        }
    }
}