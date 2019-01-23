using System.Collections.ObjectModel;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Messages
{
    public abstract class DataLockEvent : PaymentsEvent, IContractType1EarningEvent
    {
        public ReadOnlyCollection<PriceEpisode> PriceEpisodes { get; set; }
        public short CollectionYear { get; set; }
        public string AgreementId { get; set; }
        public decimal SfaContributionPercentage { get; set; }
        public ReadOnlyCollection<OnProgrammeEarning> OnProgrammeEarnings { get; set; }
        public ReadOnlyCollection<IncentiveEarning> IncentiveEarnings { get; set; }
    }
}