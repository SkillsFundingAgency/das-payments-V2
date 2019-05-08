using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class CourseValidationProcessor : ICourseValidationProcessor
    {
        private readonly List<ICourseValidator> courseValidators;
        public CourseValidationProcessor(IEnumerable<ICourseValidator> courseValidators)
        {
            this.courseValidators = new List<ICourseValidator>(courseValidators);
        }

        public CourseValidationResult ValidateCourse(DataLockValidationModel dataLockValidationModel)
        {
            var validationResults = new List<ValidationResult>();
            foreach (var courseValidator in courseValidators)
            {
                var validatorResult = courseValidator.Validate(dataLockValidationModel);
                validationResults.Add(validatorResult);
            }

            var dataLockFailures = new List<DataLockFailure>();

            var invalidApprenticeships = validationResults
                .Where(o => o.DataLockErrorCode.HasValue)
                .SelectMany(a => a.ApprenticeshipPriceEpisodes)
                .ToList();

            if (invalidApprenticeships.Any())
            {
                foreach (var invalidApprenticeship in invalidApprenticeships)
                {
                    dataLockFailures.Add(new DataLockFailure
                    {
                        MatchedPriceEpisode = invalidApprenticeship,
                        DataLockErrors = validationResults
                            .Where(x => x.ApprenticeshipPriceEpisodes.Any(o => o.Id == invalidApprenticeship.Id))
                            .Where(validationResult => validationResult.DataLockErrorCode.HasValue)
                            .Select(validationResult => validationResult.DataLockErrorCode.Value)
                            .ToList()
                    });
                }
            }

            var result = new CourseValidationResult
            {
                DataLockFailures = dataLockFailures,
                MatchedPriceEpisode = dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes
                    .Where(ape => !ape.Removed)
                    .FirstOrDefault(ape => validationResults.All(validationResult => validationResult.ApprenticeshipPriceEpisodes.Any(resultApe => resultApe.Id == ape.Id)))
            };

            return result;
        }
    }
}