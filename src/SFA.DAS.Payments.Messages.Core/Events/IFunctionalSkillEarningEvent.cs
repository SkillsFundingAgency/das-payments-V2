using System;
using System.Collections.ObjectModel;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public interface IFunctionalSkillEarningEvent : IEarningEvent
    {
        ReadOnlyCollection<FunctionalSkillEarning> Earnings { get; set; }

        DateTime StartDate { get; set; }

        ContractType ContractType { get; }
    }
}
