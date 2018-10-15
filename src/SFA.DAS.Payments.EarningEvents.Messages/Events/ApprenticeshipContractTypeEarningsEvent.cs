using System.Collections.Generic;
using System.Collections.ObjectModel;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public abstract class ApprenticeshipContractTypeEarningsEvent : EarningEvent
    {
        public decimal SfaContributionPercentage { get; set; }
        public List<OnProgrammeEarning> OnProgrammeEarnings { get; set; }
        public List<IncentiveEarning> IncentiveEarnings { get; set; }
    }
}