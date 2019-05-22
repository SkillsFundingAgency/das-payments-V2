using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class ProgrammeTypeValidator : BaseCourseValidator, ICourseValidator
    {
        protected override DataLockErrorCode DataLockerErrorCode => DataLockErrorCode.DLOCK_05;

        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(DataLockValidationModel dataLockValidationModel)
        {
            if (dataLockValidationModel.Aim.ProgrammeType == dataLockValidationModel.Apprenticeship.ProgrammeType)
            {
                return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
            }
            return new List<ApprenticeshipPriceEpisodeModel>();
        }
    }
}
