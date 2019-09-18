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

        protected override  bool FailedValidation(DataLockValidationModel dataLockValidationModel, List<ApprenticeshipPriceEpisodeModel> validApprenticeshipPriceEpisodes)
        {
            var apprenticeshipActualStartDate = GetApprenticeshipActualStartDate(dataLockValidationModel.Apprenticeship);
            var apprenticeshipActualEndDate = GetApprenticeshipEstimatedEndDate(dataLockValidationModel.Apprenticeship);

            var earningPeriodDate = calculatePeriodStartAndEndDate
                .GetPeriodDate(dataLockValidationModel.EarningPeriod.Period, dataLockValidationModel.AcademicYear);

            if (apprenticeshipActualStartDate >= earningPeriodDate.periodEndDate ||
                apprenticeshipActualEndDate <= earningPeriodDate.periodStartDate)
                return false;

            var allDuplicateApprenticeships =  dataLockLearnerCache
                .GetDuplicateApprenticeships()
                .Result;

            if (!allDuplicateApprenticeships.Any())
                return false;

            // Apprenticeship has a duplicate
            var duplicates = allDuplicateApprenticeships
                .Where(x => x.Uln == dataLockValidationModel.Apprenticeship.Uln &&
                            x.Status == ApprenticeshipStatus.Active &&
                            x.Id != dataLockValidationModel.Apprenticeship.Id &&
                            x.Ukprn != dataLockValidationModel.Apprenticeship.Ukprn)
                .ToList();

         
            if (!duplicates.Any())
                return false;
            
            foreach (var duplicate in duplicates)
            {
                var duplicateActualStartDate = GetApprenticeshipActualStartDate(duplicate);
                var duplicateActualEndDate = GetApprenticeshipEstimatedEndDate(duplicate);

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

        private static DateTime GetApprenticeshipEstimatedEndDate(ApprenticeshipModel apprenticeship)
        {
            var latestApprenticeshipPriceEpisode = apprenticeship
                .ApprenticeshipPriceEpisodes
                .Where(x => !x.Removed)
                .OrderByDescending(x => x.EndDate)
                .First();

            return latestApprenticeshipPriceEpisode.EndDate ?? DateTime.MaxValue;
        }

        private static DateTime GetApprenticeshipActualStartDate(ApprenticeshipModel apprenticeship)
        {
            var apprenticeshipActualStartDate = apprenticeship.ApprenticeshipPriceEpisodes
                .Where(x => !x.Removed)
                .OrderBy(x => x.StartDate)
                .First().StartDate;
            return apprenticeshipActualStartDate;
        }
    }

}
