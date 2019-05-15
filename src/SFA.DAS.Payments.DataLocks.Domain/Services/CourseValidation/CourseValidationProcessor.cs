using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

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
            var dataLockFailures = new List<DataLockFailure>();
            var invalidApprenticeshipPriceEpisodeIds = new List<long>();

            var allApprenticeshipPriceEpisodeIds = dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes
                .Select(x => x.Id).ToList();


            foreach (var courseValidator in courseValidators)
            {
                var validatorResult = courseValidator.Validate(dataLockValidationModel);

                if (validatorResult.DataLockErrorCode.HasValue)
                {
                    dataLockFailures.Add(new DataLockFailure
                    {
                        ApprenticeshipId = dataLockValidationModel.Apprenticeship.Id,
                        DataLockError = validatorResult.DataLockErrorCode.Value,
                        ApprenticeshipPriceEpisodeIds = allApprenticeshipPriceEpisodeIds
                    });
                }
                else
                {
                    var validApprenticeshipPriceEpisodeIds = validatorResult.ApprenticeshipPriceEpisodes
                        .Where(o => !o.Removed)
                        .Select(x => x.Id)
                        .ToList();

                    invalidApprenticeshipPriceEpisodeIds.AddRange(allApprenticeshipPriceEpisodeIds.Except(validApprenticeshipPriceEpisodeIds));
                }
            }

            var result = new CourseValidationResult();

            if (dataLockFailures.Any())
            {
                result.DataLockFailures = dataLockFailures;
            }
            else
            {
                result.MatchedPriceEpisode = dataLockValidationModel.Apprenticeship
                        .ApprenticeshipPriceEpisodes.FirstOrDefault(x =>!x.Removed && !invalidApprenticeshipPriceEpisodeIds.Contains(x.Id));
            }
            
            return result;
        }
    }
}