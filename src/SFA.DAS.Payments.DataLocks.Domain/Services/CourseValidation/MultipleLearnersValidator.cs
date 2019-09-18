using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
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
        private const int allowedOverlapDurationInMonths = 2;

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

        protected override bool FailedValidation(DataLockValidationModel dataLockValidationModel, List<ApprenticeshipPriceEpisodeModel> validApprenticeshipPriceEpisodes)
        {
            var currentApprenticeshipPriceEpisode = GetLatestApprenticeshipPriceEpisodes(dataLockValidationModel.Apprenticeship);

            var earningPeriodDate = calculatePeriodStartAndEndDate
                .GetPeriodDate(dataLockValidationModel.EarningPeriod.Period, dataLockValidationModel.AcademicYear);

            // Is Apprenticeship active within period
            if (dataLockValidationModel.Apprenticeship.EstimatedStartDate >= earningPeriodDate.periodEndDate ||
                currentApprenticeshipPriceEpisode.EndDate.HasValue && currentApprenticeshipPriceEpisode.EndDate <= earningPeriodDate.periodStartDate)
                return false;

            var allDuplicateApprenticeships = dataLockLearnerCache
                .GetDuplicateApprenticeships()
                .Result;

            if (!allDuplicateApprenticeships.Any())
                return false;

            // Apprenticeship has a duplicate
            var duplicates = allDuplicateApprenticeships
                .Where(x => x.Uln == dataLockValidationModel.Apprenticeship.Uln &&
                            x.Id != dataLockValidationModel.Apprenticeship.Id &&
                            x.Ukprn != dataLockValidationModel.Apprenticeship.Ukprn)
                .ToList();


            if (!duplicates.Any())
                return false;

            var duplicateApprenticeshipsActiveWithinPeriod = duplicates
                .Where(x => IsActiveInPeriod(x, earningPeriodDate.periodStartDate) &&
                            IsActiveInPeriod(x, earningPeriodDate.periodStartDate.AddMonths(1)))
                .ToList();


            if (!duplicateApprenticeshipsActiveWithinPeriod.Any())
            {
                return false;
            }

            foreach (var duplicate in duplicateApprenticeshipsActiveWithinPeriod)
            {
                // check if they overlap
                var duplicateEndDateToUse = duplicate.StopDate ?? duplicate.EstimatedEndDate;
                if (dataLockValidationModel.Apprenticeship.EstimatedStartDate < duplicateEndDateToUse &&
                    duplicate.EstimatedStartDate < dataLockValidationModel.Apprenticeship.EstimatedEndDate)
                {
                    return true;
                }
            }

            return false;
        }

        private ApprenticeshipPriceEpisodeModel GetLatestApprenticeshipPriceEpisodes(ApprenticeshipModel apprenticeship)
        {
            var latestApprenticeshipPriceEpisodes = apprenticeship.ApprenticeshipPriceEpisodes
                .Where(x => !x.Removed)
                .OrderByDescending(x => x.StartDate)
                .First();
            return latestApprenticeshipPriceEpisodes;
        }

        private bool IsActiveInPeriod(ApprenticeshipModel apprenticeship, DateTime periodStartDate)
        {
            var periodEndDate = periodStartDate.AddDays(-1).AddMonths(1);
            var latestApprenticeshipPriceEpisode = GetLatestApprenticeshipPriceEpisodes(apprenticeship);

            return !(apprenticeship.EstimatedStartDate >= periodEndDate ||
                    apprenticeship.EstimatedEndDate <= periodStartDate ||
                    latestApprenticeshipPriceEpisode.EndDate.HasValue &&
                    latestApprenticeshipPriceEpisode.EndDate <= periodStartDate ||
                    apprenticeship.StopDate.HasValue && apprenticeship.StopDate <= periodEndDate);

        }

    }

}
