using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{

    public interface IStartDateValidator : ICourseValidator { }

    public class StartDateValidator : BaseCourseValidator, IStartDateValidator
    {
        private readonly ICalculatePeriodStartAndEndDate calculatePeriodStartAndEndDate;


        public StartDateValidator(ICalculatePeriodStartAndEndDate calculatePeriodStartAndEndDate)
        {
            this.calculatePeriodStartAndEndDate = calculatePeriodStartAndEndDate;
        }

        protected override DataLockErrorCode DataLockerErrorCode => DataLockErrorCode.DLOCK_09;

        protected override List<ApprenticeshipPriceEpisodeModel> GetValidApprenticeshipPriceEpisodes(DataLockValidationModel dataLockValidationModel)
        {

            if (!ApprenticeshipIsWithinPeriod(dataLockValidationModel))
            {
                return new List<ApprenticeshipPriceEpisodeModel>();
            }
            
            var apprenticeshipPriceEpisodes = dataLockValidationModel.Apprenticeship.ApprenticeshipPriceEpisodes
                .Where(priceEpisode => priceEpisode.StartDate <= dataLockValidationModel.PriceEpisode.EffectiveTotalNegotiatedPriceStartDate && !priceEpisode.Removed)
                .ToList();
            return apprenticeshipPriceEpisodes;
        }

        private bool ApprenticeshipIsWithinPeriod(DataLockValidationModel dataLockValidationModel)
        {
            var periodDates = calculatePeriodStartAndEndDate
                .GetPeriodDate(dataLockValidationModel.EarningPeriod.Period, dataLockValidationModel.AcademicYear);
            return dataLockValidationModel.Apprenticeship.EstimatedStartDate <= periodDates.periodEndDate;
        }

    }
}