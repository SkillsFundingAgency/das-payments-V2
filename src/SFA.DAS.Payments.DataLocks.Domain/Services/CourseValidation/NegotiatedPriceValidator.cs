using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public interface INegotiatedPriceValidator: ICourseValidator { }

    public class NegotiatedPriceValidator : INegotiatedPriceValidator
    {
        public ValidationResult Validate(DataLockValidationModel dataLockValidationModel)
        {
            var result = new ValidationResult
            {
                ApprenticeshipId = dataLockValidationModel.ApprenticeshipId,
                Period = dataLockValidationModel.EarningPeriod.Period,
            };

            var apprenticeshipPriceEpisodes = dataLockValidationModel.ApprenticeshipPriceEpisodes
                .Where(priceEpisode => priceEpisode.Cost == dataLockValidationModel.PriceEpisode.AgreedPrice && !priceEpisode.Removed)
                .ToList();

            if (!apprenticeshipPriceEpisodes.Any())
            {
                result.DataLockErrorCode = DataLockErrorCode.DLOCK_07;
            }
            else
            {
                result.ApprenticeshipPriceEpisodes.AddRange(apprenticeshipPriceEpisodes);
            }

            return result;
        }
    }
}
