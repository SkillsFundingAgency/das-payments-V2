using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public class StartDateValidator : ICourseValidator
    {
        public List<ValidationResult> Validate(CourseValidationModel courseValidationModel)
        {
            var result = new List<ValidationResult>();

            var startDate = courseValidationModel.PriceEpisode.StartDate;

            foreach (var apprenticeshipModel in courseValidationModel.Apprenticeships)
            {
                foreach (var priceEpisode in apprenticeshipModel.ApprenticeshipPriceEpisodes)
                {
                    if (startDate < priceEpisode.StartDate)
                    {
                        result.Add(new ValidationResult
                        {
                            DataLockErrorCode = DataLockErrorCode.DLOCK_09,
                            Period = courseValidationModel.Period,
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