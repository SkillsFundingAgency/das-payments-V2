using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Model.Core
{
    public class DataLockFailure
    {
        public List<DataLockErrorCode> DataLockErrors { get; set; }
        public ApprenticeshipPriceEpisodeModel MatchedPriceEpisode { get; set; }
    }
}
