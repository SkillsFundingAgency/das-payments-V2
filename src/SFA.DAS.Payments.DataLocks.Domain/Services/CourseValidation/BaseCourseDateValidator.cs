using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public abstract class BaseCourseDateValidator
    {
        public ValidationResult Validate(DataLockValidationModel dataLockValidationModel)
        {
            var result = CreateValidationResult(dataLockValidationModel);

            var apprenticeshipPriceEpisodes = GetValidApprenticeshipPriceEpisodes(dataLockValidationModel);

            if (!apprenticeshipPriceEpisodes.Any())
            {
                SetDataLockErrorCode(result, dataLockValidationModel.Apprenticeship);
            }
            else
            {
                result.ApprenticeshipPriceEpisodes.AddRange(apprenticeshipPriceEpisodes);
            }

            return result;
        }

        protected abstract void SetDataLockErrorCode(ValidationResult result, ApprenticeshipModel apprenticeship);

        protected abstract List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes( DataLockValidationModel dataLockValidationModel);

        private static ValidationResult CreateValidationResult(DataLockValidationModel dataLockValidationModel)
        {
            var result = new ValidationResult
            {
                ApprenticeshipId = dataLockValidationModel.Apprenticeship.Id,
                Period = dataLockValidationModel.EarningPeriod.Period,
            };
            return result;
        }
    }
}