using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class CourseValidationProcessor : BaseCourseValidationProcessor, ICourseValidationProcessor
    {
        private readonly List<ICourseValidator> learnerAimValidators;

        public CourseValidationProcessor(List<ICourseValidator> courseValidators)
        {
            this.learnerAimValidators = new List<ICourseValidator>(courseValidators);
        }

        public CourseValidationResult ValidateCourse(DataLockValidationModel dataLockValidationModel)
        {
            var allApprenticeshipPriceEpisodeIds = GetAllApprenticeshipPriceEpisodeIds(dataLockValidationModel);
            var validationResults = Validate(learnerAimValidators,dataLockValidationModel, allApprenticeshipPriceEpisodeIds);
            return validationResults;
        }
       
    }
}