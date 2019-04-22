using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class ValidationResult
    {
        public DataLockErrorCode? DataLockErrorCode { get; set; }

        public long ApprenticeshipId { get; set; }

        public byte Period { get; set; }

        public List<ApprenticeshipPriceEpisodeModel> ApprenticeshipPriceEpisodes { get; set; }

        public ValidationResult()
        {
            ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>();
        }
    }
}