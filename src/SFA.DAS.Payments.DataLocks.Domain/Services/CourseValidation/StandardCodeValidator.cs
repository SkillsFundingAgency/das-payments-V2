using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class StandardCodeValidator : BaseCourseValidator, ICourseValidator
    {
        protected override DataLockErrorCode DataLockerErrorCode => DataLockErrorCode.DLOCK_03;

        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(DataLockValidationModel dataLockValidationModel)
        {
            if (dataLockValidationModel.Aim.StandardCode == dataLockValidationModel.Apprenticeship.StandardCode)
            {
                return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
            }
            return new List<ApprenticeshipPriceEpisodeModel>();
        }
    }
}
