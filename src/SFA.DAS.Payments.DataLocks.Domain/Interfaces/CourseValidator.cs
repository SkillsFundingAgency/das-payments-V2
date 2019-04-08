using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Interfaces
{
    public class CourseValidator : ICourseValidator
    {
        public Task<CourseValidationResult> ValidateCourse(CollectionPeriod priceEpisode,
            List<ApprenticeshipModel> apprenticeships)
        {
            throw new System.NotImplementedException();
        }
    }
}