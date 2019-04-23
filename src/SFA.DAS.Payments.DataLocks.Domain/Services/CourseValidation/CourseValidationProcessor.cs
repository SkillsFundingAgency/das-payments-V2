using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;

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
            var result = new CourseValidationResult
            {
                DataLockErrors = validationResults
                    .Where(validationResult => validationResult.DataLockErrorCode.HasValue)
                    .Select(validationResult => validationResult.DataLockErrorCode.Value)
                    .ToList(),
                MatchedPriceEpisode = dataLockValidationModel.ApprenticeshipPriceEpisodes
                    .Where(ape => !ape.Removed)
                    .FirstOrDefault(ape => validationResults.All(validationResult => validationResult.ApprenticeshipPriceEpisodes.Any(resultApe => resultApe.Id == ape.Id))  )
            };

            return result;
        }
    }
}