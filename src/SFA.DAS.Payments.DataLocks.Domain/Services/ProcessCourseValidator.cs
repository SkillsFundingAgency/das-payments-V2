using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public class ProcessCourseValidator : IProcessCourseValidator
    {
        private readonly List<ICourseValidator> courseValidators;

        public ProcessCourseValidator(IEnumerable<ICourseValidator> courseValidators)
        {
            this.courseValidators = new List<ICourseValidator>(courseValidators);
        }
        
        public List<ValidationResult> ValidateCourse(DataLockValidation validation)
        {
            var validationResult = new List<ValidationResult>();
            var courseValidationModel = CreateCourseValidationModel(validation);
            
            foreach (var courseValidator in courseValidators)
            {
                var results = courseValidator.Validate(courseValidationModel);

                if (results != null && results.Any())
                {
                    validationResult.AddRange(results);
                }
            }

            return validationResult;
        }

        private CourseValidation CreateCourseValidationModel(DataLockValidation validation)
        {
            return new CourseValidation
            {
                Period = validation.EarningPeriod.Period,
                PriceEpisode = validation.PriceEpisode,
                Apprenticeships = validation.Apprenticeships
            };
        }

    }
}