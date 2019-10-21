using System;
using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public interface IOnProgrammeAndIncentiveStoppedValidator : ICourseValidator { }

    public class OnProgrammeAndIncentiveStoppedValidator : BaseCourseValidator, IOnProgrammeAndIncentiveStoppedValidator
    {
        private readonly ICalculatePeriodStartAndEndDate calculatePeriodStartAndEndDate;

        public OnProgrammeAndIncentiveStoppedValidator(ICalculatePeriodStartAndEndDate calculatePeriodStartAndEndDate)
        {
            this.calculatePeriodStartAndEndDate = calculatePeriodStartAndEndDate;
        }

        protected override DataLockErrorCode DataLockerErrorCode { get; } = DataLockErrorCode.DLOCK_10;

        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(
            DataLockValidationModel dataLockValidationModel)
        {
            // Only DLOCK_10 when apprenticeship is stopped
            if (dataLockValidationModel.Apprenticeship.Status != ApprenticeshipStatus.Stopped)
            {
                return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
            }

            // Only deal with Transaction Type 1 OnProgramme and incentives
            if (dataLockValidationModel.TransactionType == TransactionType.Balancing ||
                dataLockValidationModel.TransactionType == TransactionType.Completion )
            {
                return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
            }

            var periodDate = calculatePeriodStartAndEndDate.GetPeriodDate(dataLockValidationModel.EarningPeriod.Period, dataLockValidationModel.AcademicYear);

            if (dataLockValidationModel.Apprenticeship.StopDate >= periodDate.periodEndDate)
            {
                return dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes;
            }

            return new List<ApprenticeshipPriceEpisodeModel>();
        }
        
     
    }
}
