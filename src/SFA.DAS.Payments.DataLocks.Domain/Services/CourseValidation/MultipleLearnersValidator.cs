using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class MultipleLearnersValidator : BaseCourseValidator, ICourseValidator
    {
        private readonly IDataLockLearnerCache dataLockLearnerCache;

        public MultipleLearnersValidator(IDataLockLearnerCache dataLockLearnerCache)
        {
            this.dataLockLearnerCache = dataLockLearnerCache;
        }

        protected override DataLockErrorCode DataLockerErrorCode { get; } = DataLockErrorCode.DLOCK_08;
        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(DataLockValidationModel dataLockValidationModel)
        {
            return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
        }

        protected override  bool FailedValidationAsync(DataLockValidationModel dataLockValidationModel, List<ApprenticeshipPriceEpisodeModel> validApprenticeshipPriceEpisodes)
        {
            var allDuplicateApprenticeships =  dataLockLearnerCache
                .GetDuplicateApprenticeships(dataLockValidationModel.Apprenticeship.Ukprn)
                .Result;

            if (!allDuplicateApprenticeships.Any())
                return false;

            // Apprenticeship has a duplicate
            var duplicates = allDuplicateApprenticeships.Where(x => x.Uln == dataLockValidationModel.Uln && x.Id != dataLockValidationModel.Apprenticeship.Id).ToList();

            if (!duplicates.Any())
                return false;

            // check if they have the same date range 
            var apprenticeshipModelActualStartDate = dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes.Where(x => !x.Removed)
                .OrderBy(x => x.StartDate).First().StartDate;
            var apprenticeshipModelActualEndDate = dataLockValidationModel.Apprenticeship.StopDate?? DateTime.MaxValue;

            foreach (var duplicate in duplicates)
            {
                var duplicateActualStartDate = duplicate.ApprenticeshipPriceEpisodes.Where(x => !x.Removed)
                    .OrderBy(x => x.StartDate).First().StartDate;
                var duplicateActualEndDate = duplicate.StopDate ?? DateTime.MaxValue;

                if (apprenticeshipModelActualStartDate < duplicateActualEndDate && duplicateActualStartDate < apprenticeshipModelActualEndDate)
                {
                    return true;
                }
            }
            
            return false;
        }
    }

}
