using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{

    public interface IStartDateValidator
    {
        (List<ApprenticeshipModel> validApprenticeships, List<DataLockFailure> dataLockFailures) Validate(PriceEpisode ilrPriceEpisode, List<ApprenticeshipModel> apprenticeships);
    }

    public class StartDateValidator : IStartDateValidator
    {
        public (List<ApprenticeshipModel> validApprenticeships, List<DataLockFailure> dataLockFailures) Validate(PriceEpisode ilrPriceEpisode, List<ApprenticeshipModel> apprenticeships)
        {
            var dataLockFailures = new List<DataLockFailure>();

           var  matchedApprenticeships = apprenticeships.Where(x => x.ApprenticeshipPriceEpisodes
               .Any(priceEpisode => priceEpisode.StartDate <= ilrPriceEpisode.EffectiveTotalNegotiatedPriceStartDate && !priceEpisode.Removed))
               .ToList();

            if (matchedApprenticeships.Any())
            {
                return (matchedApprenticeships, dataLockFailures);
            }

            dataLockFailures = apprenticeships.Select(a => new DataLockFailure
            {
                ApprenticeshipId = a.Id,
                ApprenticeshipPriceEpisodeIds = a.ApprenticeshipPriceEpisodes
                    .Where(o => !o.Removed)
                    .Select(x => x.Id)
                    .ToList(),
                DataLockError = DataLockErrorCode.DLOCK_09
            }).ToList();

            return  (matchedApprenticeships, dataLockFailures);
        }
    }
}