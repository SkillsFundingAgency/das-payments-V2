using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class CourseValidationResult
    {
        public List<DataLockFailure> DataLockFailures { get;set; }
        public ApprenticeshipPriceEpisodeModel MatchedPriceEpisode {  get; set; }

        public CourseValidationResult()
        {
            DataLockFailures = new List<DataLockFailure>();
        }
    }
}