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
            var validApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>();

            foreach (var courseValidator in courseValidators)
            {
                var validatorResult = courseValidator.Validate(dataLockValidationModel);
              
                if (validatorResult.DataLockErrorCode.HasValue)
                {
                    dataLockFailures.Add(new DataLockFailure
                    {
                        ApprenticeshipId =  dataLockValidationModel.Apprenticeship.Id,
                        DataLockError =  validatorResult.DataLockErrorCode.Value,
                        ApprenticeshipPriceEpisodeIds = dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes.Select(x => x.Id).ToList()
                    });
                }
                else
                {
                    var validPriceEpisodes = validatorResult.ApprenticeshipPriceEpisodes.Where(o => !o.Removed).ToList();
                    validApprenticeshipPriceEpisodes.AddRange(validPriceEpisodes);
                }
            }
            
            var result = new CourseValidationResult
            {
                DataLockFailures = dataLockFailures,
                MatchedPriceEpisode = validApprenticeshipPriceEpisodes.FirstOrDefault()
            };

            return result;
        }
    }
}