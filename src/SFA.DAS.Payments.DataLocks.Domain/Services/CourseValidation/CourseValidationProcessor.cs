using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
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

        public List<ValidationResult> ValidateCourse(DataLockValidationModel validationModel)
        {
            var validationResult = new List<ValidationResult>();

            foreach (var courseValidator in courseValidators)
            {
                var results = courseValidator.Validate(validationModel);

                if (results != null && results.Any())
                {
                    validationResult.AddRange(results);
                }
            }
            return validationResult;
        }
    }
}