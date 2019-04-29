using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class CourseValidationResult
    {
        public List< DataLockErrorCode> DataLockErrors { get;set; }
        public ApprenticeshipPriceEpisodeModel MatchedPriceEpisode {  get; set; }

        public CourseValidationResult()
        {
            DataLockErrors = new List<DataLockErrorCode>();
        }
    }
}