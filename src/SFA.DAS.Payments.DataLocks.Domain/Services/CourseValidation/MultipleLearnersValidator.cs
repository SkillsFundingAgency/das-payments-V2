using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Infrastructure;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class MultipleLearnersValidator : BaseCourseValidator, ICourseValidator
    {
        private readonly IDataLockLearnerCache dataLockLearnerCache;
        private readonly ICalculatePeriodStartAndEndDate calculatePeriodStartAndEndDate;

        public MultipleLearnersValidator(IDataLockLearnerCache dataLockLearnerCache, ICalculatePeriodStartAndEndDate calculatePeriodStartAndEndDate)
        {
            this.dataLockLearnerCache = dataLockLearnerCache;
            this.calculatePeriodStartAndEndDate = calculatePeriodStartAndEndDate;
        }

        protected override DataLockErrorCode DataLockerErrorCode { get; } = DataLockErrorCode.DLOCK_08;
        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(DataLockValidationModel dataLockValidationModel)
        {
            return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
        }

        protected override  bool FailedValidationAsync(DataLockValidationModel dataLockValidationModel, List<ApprenticeshipPriceEpisodeModel> validApprenticeshipPriceEpisodes)
        {
            var allDuplicateApprenticeships =  dataLockLearnerCache
                .GetDuplicateApprenticeships()
                .Result;

            if (!allDuplicateApprenticeships.Any())
                return false;

            // Apprenticeship has a duplicate
            var duplicates = allDuplicateApprenticeships.Where(x => x.Uln == dataLockValidationModel.Uln && x.Id != dataLockValidationModel.Apprenticeship.Id).ToList();

            if (!duplicates.Any())
                return false;
            
            var apprenticeshipActualStartDate = dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes
                .Where(x => !x.Removed)
                .OrderBy(x => x.StartDate).First().StartDate;

            var apprenticeshipActualEndDate = dataLockValidationModel.Apprenticeship.StopDate?? DateTime.MaxValue;

            var earningPeriodDate = calculatePeriodStartAndEndDate
                    .GetPeriodDate(dataLockValidationModel.EarningPeriod.Period, dataLockValidationModel.AcademicYear);

            if (apprenticeshipActualStartDate >= earningPeriodDate.periodEndDate || 
                apprenticeshipActualEndDate <= earningPeriodDate.periodStartDate)
                return false;

            foreach (var duplicate in duplicates)
            {
                var duplicateActualStartDate = duplicate.ApprenticeshipPriceEpisodes
                    .Where(x => !x.Removed)
                    .OrderBy(x => x.StartDate)
                    .First().StartDate;

                var duplicateActualEndDate = duplicate.StopDate ?? DateTime.MaxValue;

                if (duplicateActualStartDate >= earningPeriodDate.periodEndDate ||
                    duplicateActualEndDate <= earningPeriodDate.periodStartDate)
                    continue;

                // check if they have the same date range 
                if (apprenticeshipActualStartDate < duplicateActualEndDate && duplicateActualStartDate < apprenticeshipActualEndDate)
                {
                    return true;
                }
            }
            
            return false;
        }
    }

}
