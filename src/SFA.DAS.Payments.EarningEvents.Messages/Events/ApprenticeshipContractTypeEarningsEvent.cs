using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public abstract class ApprenticeshipContractTypeEarningsEvent : EarningEvent
    {
        public decimal SfaContributionPercentage { get; set; }
        public IReadOnlyCollection<OnProgrammeEarning> OnProgrammeEarnings { get; set; }
        public IReadOnlyCollection<IncentiveEarning> IncentiveEarnings { get; set; }
    }
}