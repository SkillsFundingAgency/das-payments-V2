using System;
using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    class OnProgStoppedValidator : BaseCourseValidator, ICourseValidator
    {
        protected override DataLockErrorCode DataLockerErrorCode => DataLockErrorCode.DLOCK_10;

        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(
            DataLockValidationModel dataLockValidationModel)
        {
            var censusDate = CensusDateForPeriod(dataLockValidationModel.EarningPeriod.Period, dataLockValidationModel.AcademicYear);

            if (dataLockValidationModel.EarningPeriod.)

            var apprenticeshipPriceEpisodes = dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes
                .Where(priceEpisode => priceEpisode.Cost == dataLockValidationModel.PriceEpisode.AgreedPrice && !priceEpisode.Removed)
                .ToList();

            return apprenticeshipPriceEpisodes;
        }
        
        private DateTime CensusDateForPeriod(int period, int academicYear)
        {
            return new DateTime();
        }

        private Dictionary<(int period, int academicYear), DateTime> PeriodToDate = new Dictionary<(Int32 period, Int32 academicYear), DateTime>();
    }
}
