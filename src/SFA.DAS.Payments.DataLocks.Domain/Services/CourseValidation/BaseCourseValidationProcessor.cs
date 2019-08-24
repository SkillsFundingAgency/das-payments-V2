using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class BaseCourseValidationProcessor
    {
        protected CourseValidationResult Validate(List<ICourseValidator> courseValidators, DataLockValidationModel dataLockValidationModel, List<long> allApprenticeshipPriceEpisodeIds)
        {
            var dataLockFailures = new List<DataLockFailure>();
            var invalidApprenticeshipPriceEpisodeIds = new List<long>();
            foreach (var courseValidator in courseValidators)
            {
                CheckAndAddValidationResults(
                    courseValidator,
                    dataLockValidationModel,
                    dataLockFailures,
                    allApprenticeshipPriceEpisodeIds,
                    invalidApprenticeshipPriceEpisodeIds);
            }

            var result = CreateValidationResult(dataLockValidationModel, dataLockFailures, invalidApprenticeshipPriceEpisodeIds);
            return result;
        }

        protected bool CheckAndAddValidationResults(
            ICourseValidator courseValidator,
            DataLockValidationModel dataLockValidationModel,
            List<DataLockFailure> dataLockFailures,
            List<long> allApprenticeshipPriceEpisodeIds,
            List<long> invalidApprenticeshipPriceEpisodeIds)
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

                invalidApprenticeshipPriceEpisodeIds.AddRange(
                    allApprenticeshipPriceEpisodeIds.Except(validApprenticeshipPriceEpisodeIds));
            }

            return validatorResult.DataLockErrorCode.HasValue;
        }

        protected CourseValidationResult CreateValidationResult(
            DataLockValidationModel dataLockValidationModel,
            List<DataLockFailure> dataLockFailures,
            List<long> invalidApprenticeshipPriceEpisodeIds)
        {
            var result = new CourseValidationResult();
            if (dataLockFailures.Any())
            {
                result.DataLockFailures = dataLockFailures;
            }
            else
            {
                result.MatchedPriceEpisode = dataLockValidationModel.Apprenticeship
                    .ApprenticeshipPriceEpisodes
                    .FirstOrDefault(x => !x.Removed && !invalidApprenticeshipPriceEpisodeIds.Contains(x.Id));
            }

            return result;
        }

        protected List<long> GetAllApprenticeshipPriceEpisodeIds(DataLockValidationModel dataLockValidationModel)
        {
            var allApprenticeshipPriceEpisodeIds = dataLockValidationModel
                .Apprenticeship
                .ApprenticeshipPriceEpisodes
                .Where(x => !x.Removed)
                .Select(x => x.Id)
                .ToList();

            return allApprenticeshipPriceEpisodeIds;
        }

    }
}