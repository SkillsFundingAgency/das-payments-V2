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

            var startDate = courseValidation.PriceEpisode.StartDate;

            foreach (var apprenticeshipModel in courseValidation.Apprenticeships)
            {
                foreach (var priceEpisode in apprenticeshipModel.ApprenticeshipPriceEpisodes)
                {
                    if (startDate < priceEpisode.StartDate)
                    {
                        result.Add(new ValidationResult
                        {
                            DataLockErrorCode = DataLockErrorCode.DLOCK_09,
                            Period = courseValidation.Period,
                            ApprenticeshipPriceEpisodeIdentifier = priceEpisode.Id,
                            ApprenticeshipId = apprenticeshipModel.Id
                        });
                    }
                }
            }

            return result;
        }
    }
}