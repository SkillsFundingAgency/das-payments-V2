﻿using SFA.DAS.Payments.Messages.Common;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    /// <summary>
    /// Earning events for sub aims such as Maths & English
    /// </summary>
    /// <seealso cref="EarningEvent" />
    public class Act2FunctionalSkillEarningsEvent : FunctionalSkillEarningsEvent, ILeafLevelMessage
    {
        public Act2FunctionalSkillEarningsEvent()
        {
            ContractType = ContractType.Act2;
        }
    }
}