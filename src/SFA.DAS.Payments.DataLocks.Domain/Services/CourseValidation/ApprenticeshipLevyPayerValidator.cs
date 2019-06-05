using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class ApprenticeshipLevyPayerValidator : BaseCourseValidator, ICourseValidator
    {
        protected override DataLockErrorCode DataLockerErrorCode { get; } = DataLockErrorCode.DLOCK_11;
        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(DataLockValidationModel dataLockValidationModel)
        {
            return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
        }
        protected override bool FailedValidation(DataLockValidationModel dataLockValidationModel, List<ApprenticeshipPriceEpisodeModel> validApprenticeshipPriceEpisodes)
        {
            return !dataLockValidationModel.Apprenticeship.IsLevyPayer;
        }
    }
}
