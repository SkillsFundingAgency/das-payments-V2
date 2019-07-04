using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class CompletionStoppedValidator : BaseCourseValidator, ICourseValidator
    {
        protected override DataLockErrorCode DataLockerErrorCode { get; } = DataLockErrorCode.DLOCK_10;

        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(
            DataLockValidationModel dataLockValidationModel)
        {

            // Only DLOCK_10 when apprenticeship is stopped
            if (dataLockValidationModel.Apprenticeship.Status != ApprenticeshipStatus.Stopped)
            {
                return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
            }

            // Only deal with Transactin Type 2 & 3 (Completion and balancing)
            if (dataLockValidationModel.TransactionType != TransactionType.Completion &&
                dataLockValidationModel.TransactionType != TransactionType.Balancing)
            {
                return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
            }

            if (dataLockValidationModel.PriceEpisode.ActualEndDate <= dataLockValidationModel.Apprenticeship.StopDate)
            {
                return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
            }

            return new List<ApprenticeshipPriceEpisodeModel>();
        }
    }
}
