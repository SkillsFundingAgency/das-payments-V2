using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public interface IContractTypeEarningEvent : IEarningEvent
    {
        decimal SfaContributionPercentage { get; }
        List<OnProgrammeEarning> OnProgrammeEarnings { get; }
        List<IncentiveEarning> IncentiveEarnings { get;  }
    }
}
