using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Model.Core
{
    public class DataLockFailure
    {
        public ApprenticeshipModel Apprenticeship { get; set; }
        public DataLockErrorCode DataLockError { get; set; }
        public List<ApprenticeshipPriceEpisodeModel> ApprenticeshipPriceEpisodes { get; set; }
    }
}
