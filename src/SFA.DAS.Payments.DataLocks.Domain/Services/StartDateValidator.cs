using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public class StartDateValidator : ICourseValidator
    {
        public List<ValidationResult> Validate(DataLockValidationModel dataLockValidationModel)
        {
            var result = new List<ValidationResult>();

            var startDate = dataLockValidationModel.PriceEpisode.StartDate;
            var apprenticeship = dataLockValidationModel.Apprenticeship;

                foreach (var priceEpisode in apprenticeship.ApprenticeshipPriceEpisodes)
                {
                    if (startDate < priceEpisode.StartDate)
                    {
                        result.Add(new ValidationResult
                        {
                            DataLockErrorCode = DataLockErrorCode.DLOCK_09,
                            Period = dataLockValidationModel.EarningPeriod.Period,
                            ApprenticeshipPriceEpisodeIdentifier = priceEpisode.Id,
                            ApprenticeshipId = apprenticeship.Id
                        });
                    }
                }
            

            return result;
        }
    }
}