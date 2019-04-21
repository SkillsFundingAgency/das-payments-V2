using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public class ApprenticeshipPauseValidator : ICourseValidator
    {
        public List<ValidationResult> Validate(DataLockValidationModel validationModel)
        {
            var result = new List<ValidationResult>();
            var apprenticeship = validationModel.Apprenticeship;
         
                if (apprenticeship.Status == ApprenticeshipPaymentStatus.Paused)
                {
                    foreach(var priceEpisode in apprenticeship.ApprenticeshipPriceEpisodes)
                    {
                        result.Add(new ValidationResult
                        {
                            DataLockErrorCode = DataLockErrorCode.DLOCK_12,
                            Period = validationModel.EarningPeriod.Period,
                            ApprenticeshipPriceEpisodeIdentifier = priceEpisode.Id,
                            ApprenticeshipId = apprenticeship.Id
                        });
                    }
                }
            return result;
        }
    }
}