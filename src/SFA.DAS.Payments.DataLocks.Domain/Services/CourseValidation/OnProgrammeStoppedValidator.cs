using System;
using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class OnProgrammeStoppedValidator : BaseCourseValidator, ICourseValidator
    {
        protected override DataLockErrorCode DataLockerErrorCode { get; } = DataLockErrorCode.DLOCK_10;

        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(
            DataLockValidationModel dataLockValidationModel)
        {
            // Only DLOCK_10 when apprenticeship is stopped
            if (dataLockValidationModel.Apprenticeship.Status != ApprenticeshipStatus.Stopped)
            {
                return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
            }

            // Only deal with Transactin Type 1 (On Programme)
            if (dataLockValidationModel.TransactionType != OnProgrammeEarningType.Learning)
            {
                return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
            }

            var censusDate = CensusDateForPeriod(dataLockValidationModel.EarningPeriod.Period, dataLockValidationModel.AcademicYear);

            if (dataLockValidationModel.Apprenticeship.StopDate >= censusDate)
            {
                return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
            }

            return new List<ApprenticeshipPriceEpisodeModel>();
        }
        
        private DateTime CensusDateForPeriod(int period, int academicYear)
        {
            var calendarMonth = (period < 6) ? period + 7 : period - 5;
            var year = (academicYear % 100) + 2000; // the second part of the academic year in yyyy format

            if (period < 6)
            {
                year--;
            }

            var day = DateTime.DaysInMonth(year, calendarMonth);
            var periodDate = new DateTime(year, calendarMonth, day);
            
            return periodDate;
        }
    }
}
