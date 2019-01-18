using System.Collections.ObjectModel;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Messages
{
    public interface IDataLockableEarningEvent : IEarningEvent
    {
        string AgreementId { get; set; }
        decimal SfaContributionPercentage { get; set; }
        ReadOnlyCollection<OnProgrammeEarning> OnProgrammeEarnings { get; set; }
        ReadOnlyCollection<IncentiveEarning> IncentiveEarnings { get; set; }
    }
}