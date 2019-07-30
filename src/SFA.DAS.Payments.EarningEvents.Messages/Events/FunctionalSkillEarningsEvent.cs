using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    [KnownType("GetInheritors")]
    public abstract class FunctionalSkillEarningsEvent : EarningEvent, IFunctionalSkillEarningEvent
    {
        /// <summary>
        /// Gets or sets the earnings.
        /// </summary>
        /// <value>
        /// The earnings.
        /// </value>
        /// <seealso cref="SFA.DAS.Payments.Model.Core.Incentives.FunctionalSkillEarning" />
        /// <seealso cref="SFA.DAS.Payments.Model.Core.Incentives.FunctionalSkillType" />
        public ReadOnlyCollection<FunctionalSkillEarning> Earnings { get; set; }

        public DateTime StartDate { get; set; }

        public ContractType ContractType { get; set; }

        private static Type[] inheritors;

        private static Type[] GetInheritors()
        {
            return inheritors ?? (inheritors = typeof(FunctionalSkillEarningsEvent).Assembly.GetTypes()
                       .Where(x => x.IsSubclassOf(typeof(FunctionalSkillEarningsEvent)))
                       .ToArray());
        }
    }
}