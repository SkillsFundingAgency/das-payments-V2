using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class PathwayCodeValidator : BaseCourseValidator, ICourseValidator
    {
        protected override DataLockErrorCode DataLockerErrorCode => DataLockErrorCode.DLOCK_06;

        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(DataLockValidationModel dataLockValidationModel)
        {
            if (dataLockValidationModel.Aim.PathwayCode == dataLockValidationModel.Apprenticeship.PathwayCode)
            {
                return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
            }
            return new List<ApprenticeshipPriceEpisodeModel>();
        }
    }
}
