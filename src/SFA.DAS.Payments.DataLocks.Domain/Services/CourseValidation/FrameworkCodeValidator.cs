using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class FrameworkCodeValidator : BaseCourseValidator, ICourseValidator
    {
        protected override DataLockErrorCode DataLockerErrorCode => DataLockErrorCode.DLOCK_04;

        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(DataLockValidationModel dataLockValidationModel)
        {
            if (dataLockValidationModel.Aim.FrameworkCode == dataLockValidationModel.Apprenticeship.FrameworkCode)
            {
                return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
            }
            return new List<ApprenticeshipPriceEpisodeModel>();
        }
    }
}
