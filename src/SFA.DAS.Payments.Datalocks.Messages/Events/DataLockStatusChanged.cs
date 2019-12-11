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
    public class PriceEpisodeStatusChange
    {
        public decimal AgreedPrice { get; set; }
        public LegacyDataLockEvent DataLock { get; set; }
        public LegacyDataLockEventCommitmentVersion[] CommitmentVersions { get; set; }
        public LegacyDataLockEventError[] Errors { get; set; }
        public LegacyDataLockEventPeriod[] Periods { get; set; }
    }

}
