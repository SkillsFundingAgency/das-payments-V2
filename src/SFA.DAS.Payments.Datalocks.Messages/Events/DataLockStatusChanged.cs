using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class PriceEpisodeStatusChange : PaymentsEvent
    {
        public LegacyDataLockEvent DataLock { get; set; }
        public LegacyDataLockEventCommitmentVersion[] CommitmentVersions { get; set; }
        public LegacyDataLockEventError[] Errors { get; set; }
        public LegacyDataLockEventPeriod[] Period { get; set; }
    }

    [KnownType("GetInheritors")]
    [Obsolete]
    public /*abstract*/ class DataLockStatusChanged : PaymentsEvent
    {
        public Dictionary<TransactionType, List<EarningPeriod>> TransactionTypesAndPeriods { get; set; }

        public List<PriceEpisode> PriceEpisodes { get; set; } // TODO remove
        public List<PriceEpisodeStatusChange> PriceEpisodeStatusChanges { get; set; }

        private static Type[] inheritors;
        private static Type[] GetInheritors()
        {
            return inheritors ?? (inheritors = typeof(DataLockStatusChanged).Assembly.GetTypes()
                       .Where(x => x.IsSubclassOf(typeof(DataLockStatusChanged)))
                       .ToArray());
        }
    }
}
