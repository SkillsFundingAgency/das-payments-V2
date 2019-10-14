using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public abstract class ApprenticeshipContractTypeEarningsEvent : EarningEvent, IContractTypeEarningEvent
    {
        public virtual bool IsPayable => true;
        public decimal SfaContributionPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public List<OnProgrammeEarning> OnProgrammeEarnings { get; set; }
        public List<IncentiveEarning> IncentiveEarnings { get; set; }
    }
}