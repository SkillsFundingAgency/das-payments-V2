using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class NegotiatedPriceValidator : BaseCourseDateValidator,  ICourseValidator
    {
        protected override void SetDataLockErrorCode(ValidationResult result, ApprenticeshipModel apprenticeship)
        {
            result.DataLockErrorCode = DataLockErrorCode.DLOCK_07;
        }

        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(DataLockValidationModel dataLockValidationModel)
        {
            var apprenticeshipPriceEpisodes = dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes
                .Where(priceEpisode => priceEpisode.Cost == dataLockValidationModel.PriceEpisode.AgreedPrice && !priceEpisode.Removed)
                .ToList();

            return apprenticeshipPriceEpisodes;
        }
    }
}
