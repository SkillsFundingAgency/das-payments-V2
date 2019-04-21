using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public class NegotiatedPriceValidator : ICourseValidator
    {
        public List<ValidationResult> Validate(DataLockValidationModel courseValidationModel)
        {
            var result = new List<ValidationResult>();

            var ilrAgreedPrice = courseValidationModel.PriceEpisode.AgreedPrice;
            var apprenticeship = courseValidationModel.Apprenticeship;


                foreach (var apprenticeshipPriceEpisodeModel in apprenticeship.ApprenticeshipPriceEpisodes)
                {
                    if (ilrAgreedPrice == apprenticeshipPriceEpisodeModel.Cost)
                    {
                        return new List<ValidationResult>();
                    }
                   
                    result.Add(new ValidationResult
                    {
                        DataLockErrorCode = DataLockErrorCode.DLOCK_07,
                        Period = courseValidationModel.EarningPeriod.Period,
                        ApprenticeshipPriceEpisodeIdentifier = apprenticeshipPriceEpisodeModel.Id,
                        ApprenticeshipId = apprenticeship.Id
                    });
                }
           

            return result;
        }
    }
}
