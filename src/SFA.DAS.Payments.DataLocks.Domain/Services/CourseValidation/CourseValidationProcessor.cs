using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;

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
        
        private (List<DataLockFailure> dataLockFailures, List<long> invalidApprenticeshipPriceEpisodeIds) Validate(
            ICourseValidator courseValidator, DataLockValidationModel dataLockValidationModel, List<long> allApprenticeshipPriceEpisodeIds)
        {
            var dataLockFailures = new List<DataLockFailure>();
            var invalidApprenticeshipPriceEpisodeIds = new List<long>();

            CheckAndAddValidationResults(
                courseValidator,
                 dataLockValidationModel,
                 dataLockFailures,
                 allApprenticeshipPriceEpisodeIds,
                 invalidApprenticeshipPriceEpisodeIds);

            return (dataLockFailures, invalidApprenticeshipPriceEpisodeIds);
        }
    }
}