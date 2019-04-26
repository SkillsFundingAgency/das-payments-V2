using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class StartDateValidator : BaseCourseDateValidator, ICourseValidator
    {
        protected override void SetDataLockErrorCode(ValidationResult result, ApprenticeshipModel apprenticeship)
        {
            result.DataLockErrorCode = DataLockErrorCode.DLOCK_09;
        }

        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(DataLockValidationModel dataLockValidationModel)
        {
            var apprenticeshipPriceEpisodes = dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes
                .Where(priceEpisode => priceEpisode.StartDate <= dataLockValidationModel.PriceEpisode.StartDate && !priceEpisode.Removed)
                .ToList();
            return apprenticeshipPriceEpisodes;
        }
    }
}