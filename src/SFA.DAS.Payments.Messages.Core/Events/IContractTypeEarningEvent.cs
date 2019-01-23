using System.Collections.ObjectModel;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public interface IContractTypeEarningEvent : IEarningEvent
    {
        decimal SfaContributionPercentage { get; }
        ReadOnlyCollection<OnProgrammeEarning> OnProgrammeEarnings { get; }
        ReadOnlyCollection<IncentiveEarning> IncentiveEarnings { get;  }
    }

    public interface IContractType1EarningEvent : IContractTypeEarningEvent
    {
        string AgreementId { get; }
    }
}
