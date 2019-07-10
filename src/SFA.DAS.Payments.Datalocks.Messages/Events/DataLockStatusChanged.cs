using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    [KnownType("GetInheritors")]
    public abstract class DataLockStatusChanged : PaymentsEvent
    {
        public Dictionary<TransactionType, List<EarningPeriod>> TransactionTypesAndPeriods { get; set; }
        
        private static Type[] inheritors;
        private static Type[] GetInheritors()
        {
            return inheritors ?? (inheritors = typeof(DataLockEvent).Assembly.GetTypes()
                       .Where(x => x.IsSubclassOf(typeof(DataLockEvent)))
                       .ToArray());
        }
    }
}
