using System.Collections.ObjectModel;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public abstract class ApprenticeshipContractTypeEarningsEvent : EarningEvent
    {
        public decimal SfaContributionPercentage { get; set; }
        public ReadOnlyCollection<OnProgrammeEarning> OnProgrammeEarnings { get; set; }
        public ReadOnlyCollection<IncentiveEarning> IncentiveEarnings { get; set; }
    }
}