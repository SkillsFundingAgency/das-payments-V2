using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public abstract class BaseCourseValidator
    {
        protected abstract DataLockErrorCode DataLockerErrorCode { get; } 
        public ValidationResult Validate(DataLockValidationModel dataLockValidationModel)
        {
            var result = CreateValidationResult(dataLockValidationModel);

             var validApprenticeshipPriceEpisodes = GetValidApprenticeshipPriceEpisodes(dataLockValidationModel);

            if (FailedValidation(dataLockValidationModel.Apprenticeship.Status, validApprenticeshipPriceEpisodes))
            {
                result.DataLockErrorCode = DataLockerErrorCode;
            }
            else
            {
                result.ApprenticeshipPriceEpisodes.AddRange(validApprenticeshipPriceEpisodes);
            }
            return result;
        }

        protected abstract List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes( DataLockValidationModel dataLockValidationModel);

        private ValidationResult CreateValidationResult(DataLockValidationModel dataLockValidationModel)
        {
            var result = new ValidationResult
            {
                ApprenticeshipId = dataLockValidationModel.Apprenticeship.Id,
                Period = dataLockValidationModel.EarningPeriod.Period,
            };
            return result;
        }

        protected virtual bool FailedValidation(ApprenticeshipStatus  apprenticeshipStatus, List<ApprenticeshipPriceEpisodeModel> apprenticeshipPriceEpisodes)
        {
            return !apprenticeshipPriceEpisodes.Any();
        }

    }
}