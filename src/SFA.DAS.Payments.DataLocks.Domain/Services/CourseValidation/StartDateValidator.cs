using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class StartDateValidator : ICourseValidator
    {
        public ValidationResult Validate(DataLockValidationModel dataLockValidationModel)
        {
            var result = new ValidationResult
            {
                ApprenticeshipId = dataLockValidationModel.Apprenticeship.Id,
                Period = dataLockValidationModel.EarningPeriod.Period,
            };

            var apprenticeshipPriceEpisode = dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes?
                .LastOrDefault(priceEpisode => priceEpisode.StartDate <= dataLockValidationModel.PriceEpisode.StartDate && !priceEpisode.Removed);

            if (apprenticeshipPriceEpisode == null)
            {
                result.DataLockErrorCode = DataLockErrorCode.DLOCK_09;
            }
            else
            {
                result.ApprenticeshipPriceEpisodeIdentifier = apprenticeshipPriceEpisode.Id;
            }

            return result;
        }
    }
}