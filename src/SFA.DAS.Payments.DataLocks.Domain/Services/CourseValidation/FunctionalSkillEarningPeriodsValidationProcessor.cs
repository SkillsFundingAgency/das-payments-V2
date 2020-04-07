using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public interface IFunctionalSkillEarningPeriodsValidationProcessor
    {
        (List<EarningPeriod> ValidPeriods, List<EarningPeriod> InValidPeriods) ValidatePeriods(
            long ukprn,
            long uln,
            List<EarningPeriod> periods,
            TransactionType transactionType,
            List<ApprenticeshipModel> apprenticeships,
            LearningAim aim,
            int academicYear,
            bool disableDatalocks);
    }

    public class FunctionalSkillEarningPeriodsValidationProcessor : BaseEarningPeriodsValidationProcessor , IFunctionalSkillEarningPeriodsValidationProcessor
    {
        private readonly IFunctionalSkillValidationProcessor functionalSkillValidationProcessor;
        private readonly ICalculatePeriodStartAndEndDate calculatePeriodStartAndEndDate;

        public FunctionalSkillEarningPeriodsValidationProcessor(
            IFunctionalSkillValidationProcessor functionalSkillValidationProcessor,
            ICalculatePeriodStartAndEndDate calculatePeriodStartAndEndDate)
        {
            this.functionalSkillValidationProcessor = functionalSkillValidationProcessor;
            this.calculatePeriodStartAndEndDate = calculatePeriodStartAndEndDate;
        }

        protected override (List<ApprenticeshipModel> validApprenticeships, List<DataLockFailure> dataLockFailures) ValidateApprenticeships(
            long ukprn, 
            List<ApprenticeshipModel> apprenticeships,
            int academicYear, 
            EarningPeriod period, 
            TransactionType transactionType,
            List<PriceEpisode> priceEpisodes,
            bool disableDatalocks)
        {
            var apprenticeshipsToUseThisPeriod = GetApprenticeshipsToUseThisPeriod(ukprn, apprenticeships, academicYear, period);
            return (apprenticeshipsToUseThisPeriod, new List<DataLockFailure>());
        }

        protected override CourseValidationResult ValidateApprenticeship(
            long uln,
            TransactionType transactionType,
            LearningAim aim,
            int academicYear, 
            EarningPeriod period, 
            ApprenticeshipModel apprenticeship,
            List<PriceEpisode> priceEpisodes = null)
        {
            var validationModel = new DataLockValidationModel
            {
                EarningPeriod = period,
                Apprenticeship = apprenticeship,
                TransactionType = transactionType,
                Aim = aim,
                AcademicYear = academicYear
            };

            return functionalSkillValidationProcessor.ValidateCourse(validationModel);
        }

        public (List<EarningPeriod> ValidPeriods, List<EarningPeriod> InValidPeriods) ValidatePeriods(
            long ukprn,
            long uln,
            List<EarningPeriod> periods,
            TransactionType transactionType,
            List<ApprenticeshipModel> apprenticeships,
            LearningAim aim,
            int academicYear,
            bool disableDatalocks)
        {
            return base.ValidateEarningPeriods(ukprn, uln, periods, transactionType,
                apprenticeships, aim, academicYear, disableDatalocks);
        }


        private List<ApprenticeshipModel> GetApprenticeshipsToUseThisPeriod(long ukprn, List<ApprenticeshipModel> apprenticeships, int academicYear, EarningPeriod period)
        {
            var apprenticeshipsToUseThisPeriod = new List<ApprenticeshipModel>();

            var apprenticeshipsWithinPeriod = apprenticeships
                .Where(a => a.Ukprn == ukprn && ApprenticeshipStartWithinPeriod(a, period.Period, academicYear))
                .ToList();

            var activeApprenticeships =
                apprenticeshipsWithinPeriod
                    .Where(x => x.Status == ApprenticeshipStatus.Active)
                    .ToList();

            if (!activeApprenticeships.Any())
            {
                var mostRecentApprenticeship = apprenticeshipsWithinPeriod
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();
                if (mostRecentApprenticeship != null)
                {
                    apprenticeshipsToUseThisPeriod.Add(mostRecentApprenticeship);
                }
            }
            else
            {
                var mostRecentApprenticeship = activeApprenticeships
                    .OrderByDescending(x => x.EstimatedStartDate)
                    .FirstOrDefault();

                apprenticeshipsToUseThisPeriod.Add(mostRecentApprenticeship);
            }

            if (apprenticeshipsToUseThisPeriod.Count == 0)
            {
                apprenticeshipsToUseThisPeriod = apprenticeships
                    .OrderByDescending(x => x.EstimatedStartDate)
                    .Take(1)
                    .ToList();
            }
            
            return apprenticeshipsToUseThisPeriod;
        }

        private bool ApprenticeshipStartWithinPeriod(ApprenticeshipModel apprenticeship, byte deliveryPeriod, int academicYear)
        {
            var periodDates = calculatePeriodStartAndEndDate.GetPeriodDate(deliveryPeriod, academicYear);

            if (apprenticeship.EstimatedStartDate > periodDates.periodEndDate)
            {
                return false;
            }

            return true;
        }

    }
}