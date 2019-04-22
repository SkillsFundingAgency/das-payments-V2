using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class DataLockValidationModel
    {
        public long Uln { get; set; }
        public PriceEpisode PriceEpisode { get; set; }
        public EarningPeriod EarningPeriod { get; set; }
        public long ApprenticeshipId { get; set; }
        public List<ApprenticeshipPriceEpisodeModel> ApprenticeshipPriceEpisodes { get; set; }
        public ApprenticeshipStatus ApprenticeshipStatus { get; set; }
        public DataLockValidationModel()
        {
            ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>();
        }

    }
}
