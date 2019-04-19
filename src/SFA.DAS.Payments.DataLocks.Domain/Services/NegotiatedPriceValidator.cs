using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public class NegotiatedPriceValidator : ICourseValidator
    {
        public List<ValidationResult> Validate(CourseValidationModel courseValidationModel)
        {
            var result = new List<ValidationResult>();

            var ilrAgreedPrice = courseValidationModel.PriceEpisode.AgreedPrice;

            foreach (var apprenticeshipModel in courseValidationModel.Apprenticeships)
            {
                foreach (var apprenticeshipPriceEpisodeModel in apprenticeshipModel.ApprenticeshipPriceEpisodes)
                {
                    if (ilrAgreedPrice == apprenticeshipPriceEpisodeModel.Cost) break;
                   
                    result.Add(new ValidationResult
                    {
                        DataLockErrorCode = DataLockErrorCode.DLOCK_07,
                        Period = courseValidationModel.Period,
                        ApprenticeshipPriceEpisodeIdentifier = apprenticeshipPriceEpisodeModel.Id,
                        ApprenticeshipId = apprenticeshipModel.Id
                    });
                }
            }

            return result;
        }
    }
}
