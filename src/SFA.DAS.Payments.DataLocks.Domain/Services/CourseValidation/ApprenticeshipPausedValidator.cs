using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public interface IApprenticeshipPauseValidator: ICourseValidator { }

    public class ApprenticeshipPauseValidator : IApprenticeshipPauseValidator
    {
        public ValidationResult Validate(DataLockValidationModel dataLockValidationModel)
        {
            var result = new ValidationResult
            {
                ApprenticeshipId = dataLockValidationModel.ApprenticeshipId,
                Period = dataLockValidationModel.EarningPeriod.Period,
            };

            if (dataLockValidationModel.ApprenticeshipStatus == ApprenticeshipStatus.Paused)
            {
                result.DataLockErrorCode = DataLockErrorCode.DLOCK_12;
            }
            else
            {
                result.ApprenticeshipPriceEpisodes.AddRange(dataLockValidationModel.ApprenticeshipPriceEpisodes);
            }

            return result;
        }
    }
}