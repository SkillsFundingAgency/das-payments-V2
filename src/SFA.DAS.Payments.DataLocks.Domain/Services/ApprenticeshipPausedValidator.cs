using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public class ApprenticeshipPauseValidator : ICourseValidator
    {
        public List<ValidationResult> Validate(CourseValidationModel validationModel)
        {
            var result = new List<ValidationResult>();
            foreach (var apprenticeshipModel in validationModel.Apprenticeships)
            {
                if (apprenticeshipModel.Status == ApprenticeshipPaymentStatus.Paused)
                {
                    foreach(var priceEpisode in apprenticeshipModel.ApprenticeshipPriceEpisodes)
                    {
                        result.Add(new ValidationResult
                        {
                            DataLockErrorCode = DataLockErrorCode.DLOCK_12,
                            Period = validationModel.Period,
                            ApprenticeshipPriceEpisodeIdentifier = priceEpisode.Id,
                            ApprenticeshipId = apprenticeshipModel.Id
                        });
                    }
                }
            }

            return result;
        }
    }
}