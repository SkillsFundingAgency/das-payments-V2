using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class CourseValidationProcessor : ICourseValidationProcessor
    {
        private readonly List<ICourseValidator> courseValidators;

        public CourseValidationProcessor(IStartDateValidator startDateValidator, INegotiatedPriceValidator negotiatedPriceValidator, IApprenticeshipPauseValidator apprenticeshipPauseValidator)
        {
            if (startDateValidator == null) throw new ArgumentNullException(nameof(startDateValidator));
            if (negotiatedPriceValidator == null) throw new ArgumentNullException(nameof(negotiatedPriceValidator));
            if (apprenticeshipPauseValidator == null)
                throw new ArgumentNullException(nameof(apprenticeshipPauseValidator));
            courseValidators = new List<ICourseValidator> { startDateValidator, negotiatedPriceValidator, apprenticeshipPauseValidator };
        }

        public List<ValidationResult> ValidateCourse(DataLockValidationModel validationModel)
        {
            var validationResult = new List<ValidationResult>();

            foreach (var courseValidator in courseValidators)
            {
                var validatorResult = courseValidator.Validate(validationModel);
                validationResult.Add(validatorResult);
            }
            return validationResult;
        }
    }
}