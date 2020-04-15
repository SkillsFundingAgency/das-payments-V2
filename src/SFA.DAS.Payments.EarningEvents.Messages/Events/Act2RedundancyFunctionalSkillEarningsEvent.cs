using System;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public class Act2RedundancyFunctionalSkillEarningsEvent : FunctionalSkillEarningsEvent,IRedundancyEarningEvent
    {
        public Act2RedundancyFunctionalSkillEarningsEvent()
        {
            ContractType = ContractType.Act2;
            EventId = Guid.NewGuid();
        }
    }
}