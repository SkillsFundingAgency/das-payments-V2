using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public interface IStartDateValidator : ICourseValidator
    {

    }

    public class StartDateValidator : IStartDateValidator
    {
        public ValidationResult Validate(DataLockValidationModel dataLockValidationModel)
        {
            var result = new ValidationResult
            {
                ApprenticeshipId = dataLockValidationModel.ApprenticeshipId,
                Period = dataLockValidationModel.EarningPeriod.Period,
            };

            var apprenticeshipPriceEpisodes = dataLockValidationModel.ApprenticeshipPriceEpisodes
                .Where(priceEpisode => priceEpisode.StartDate <= dataLockValidationModel.PriceEpisode.StartDate && !priceEpisode.Removed)
                .ToList();

            if (!apprenticeshipPriceEpisodes.Any())
            {
                result.DataLockErrorCode = DataLockErrorCode.DLOCK_09;
            }
            else
            {
                result.ApprenticeshipPriceEpisodes.AddRange(apprenticeshipPriceEpisodes);
            }

            return result;
        }
    }
}