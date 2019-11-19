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
    //public class PriceEpisodeStatus : PriceEpisode
    //{
    //    public long Status { get; set; }
    //}

    public class PriceEpisodeStatusChange
    {
        public LegacyDataLockEvent DataLock { get; set; }
    }

    [KnownType("GetInheritors")]
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
