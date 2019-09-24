using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class FunctionalSkillValidationProcessor: BaseCourseValidationProcessor, IFunctionalSkillValidationProcessor
    {
        private readonly List<ICourseValidator> courseValidators;

        public FunctionalSkillValidationProcessor(IEnumerable<ICourseValidator> courseValidators)
        {
            this.courseValidators = courseValidators.ToList();
        }

        public CourseValidationResult ValidateCourse(DataLockValidationModel validationModel)
        {
            var allApprenticeshipPriceEpisodeIds = GetAllApprenticeshipPriceEpisodeIds(validationModel);
            var validationResult = Validate(courseValidators, validationModel, allApprenticeshipPriceEpisodeIds);
            return validationResult;
        }
    }
}