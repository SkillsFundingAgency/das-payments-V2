using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Model.Core
{
    public class DataLockFailure
    {
        public long? ApprenticeshipId { get; set; }
        public DataLockErrorCode DataLockError { get; set; }
        public List<long> ApprenticeshipPriceEpisodeIds { get; set; }
    }
}
