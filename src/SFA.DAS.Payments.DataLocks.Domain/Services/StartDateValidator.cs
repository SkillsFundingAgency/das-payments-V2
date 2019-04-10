using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public class StartDateValidator : ICourseValidator
    {
        public List<ValidationResult> Validate(CourseValidation courseValidation)
        {
            var result = new List<ValidationResult>();

            foreach (var apprenticeshipModel in courseValidation.Apprenticeships)
            {
                if (courseValidation.StartDate < apprenticeshipModel.EstimatedStartDate ||
                    courseValidation.StartDate > apprenticeshipModel.EstimatedEndDate)
                {
                    result.Add(new ValidationResult
                    {
                        DataLockErrorCode = DataLockErrorCode.DLOCK_09,
                        Period = courseValidation.Period,
                        ApprenticeshipPriceEpisodeIdentifier = courseValidation.PriceEpisodeIdentifier,
                        ApprenticeshipId = apprenticeshipModel.Id
                    });
                }
            }

            return result;
        }
    }
}