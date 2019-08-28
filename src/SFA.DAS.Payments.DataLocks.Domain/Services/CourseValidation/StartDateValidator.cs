using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{

    public interface IStartDateValidator : ICourseValidator { }

    public class StartDateValidator : BaseCourseValidator, IStartDateValidator
    {
        protected override DataLockErrorCode DataLockerErrorCode => DataLockErrorCode.DLOCK_09;

        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(DataLockValidationModel dataLockValidationModel)
        {
            var apprenticeshipPriceEpisodes = dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes
                .Where(priceEpisode => priceEpisode.StartDate <= dataLockValidationModel.PriceEpisode.EffectiveTotalNegotiatedPriceStartDate && !priceEpisode.Removed)
                .ToList();
            return apprenticeshipPriceEpisodes;
        }
    }
}