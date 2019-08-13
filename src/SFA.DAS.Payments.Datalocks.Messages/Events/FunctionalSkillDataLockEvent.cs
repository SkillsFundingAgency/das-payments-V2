using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    [KnownType("GetInheritors")]
    public abstract class FunctionalSkillDataLockEvent : DataLockEvent, IFunctionalSkillEarningEvent
    {
        public ReadOnlyCollection<FunctionalSkillEarning> Earnings { get; set; }
        public DateTime StartDate { get; set; }

        public ContractType ContractType { get; set; }

        private static Type[] inheritors;
        private static Type[] GetInheritors()
        {
            return inheritors ?? (inheritors = typeof(FunctionalSkillDataLockEvent).Assembly.GetTypes()
                       .Where(x => x.IsSubclassOf(typeof(FunctionalSkillDataLockEvent)))
                       .ToArray());
        }
    }
}
