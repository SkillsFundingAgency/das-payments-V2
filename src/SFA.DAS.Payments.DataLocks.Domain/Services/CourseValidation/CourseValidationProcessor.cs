using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class CourseValidationProcessor : BaseCourseValidationProcessor,ICourseValidationProcessor
    {
        private readonly IStartDateValidator startDateValidator;
        private readonly ICompletionStoppedValidator completionStoppedValidator;
        private readonly IOnProgrammeAndIncentiveStoppedValidator onProgrammeAndIncentiveStoppedValidator;
        private readonly List<ICourseValidator> learnerAimValidators;

        public CourseValidationProcessor(IStartDateValidator startDateValidator,
            ICompletionStoppedValidator completionStoppedValidator,
            IOnProgrammeAndIncentiveStoppedValidator onProgrammeAndIncentiveStoppedValidator,
            List<ICourseValidator> courseValidators)
        {
            this.startDateValidator = startDateValidator;
            this.completionStoppedValidator = completionStoppedValidator;
            this.onProgrammeAndIncentiveStoppedValidator = onProgrammeAndIncentiveStoppedValidator;
            this.learnerAimValidators = new List<ICourseValidator>(courseValidators);
        }

        public CourseValidationResult ValidateCourse(DataLockValidationModel dataLockValidationModel)
        {
            var allApprenticeshipPriceEpisodeIds = GetAllApprenticeshipPriceEpisodeIds(dataLockValidationModel);

            var startDateValidationResult = Validate(startDateValidator, dataLockValidationModel, allApprenticeshipPriceEpisodeIds);
            if (startDateValidationResult.dataLockFailures.Any())
            {
                return CreateValidationResult(dataLockValidationModel,
                    startDateValidationResult.dataLockFailures,
                    startDateValidationResult.invalidApprenticeshipPriceEpisodeIds);
            }

            var completionStoppedValidationResult = Validate(completionStoppedValidator, dataLockValidationModel, allApprenticeshipPriceEpisodeIds);
            if (completionStoppedValidationResult.dataLockFailures.Any())
            {
                return CreateValidationResult(dataLockValidationModel,
                    completionStoppedValidationResult.dataLockFailures,
                    completionStoppedValidationResult.invalidApprenticeshipPriceEpisodeIds);
            }

            var onProgrammeAndIncentiveStoppedValidationResult = Validate(onProgrammeAndIncentiveStoppedValidator, dataLockValidationModel, allApprenticeshipPriceEpisodeIds);
            if (onProgrammeAndIncentiveStoppedValidationResult.dataLockFailures.Any())
            {
                return CreateValidationResult(dataLockValidationModel,
                    onProgrammeAndIncentiveStoppedValidationResult.dataLockFailures,
                    onProgrammeAndIncentiveStoppedValidationResult.invalidApprenticeshipPriceEpisodeIds);
            }

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