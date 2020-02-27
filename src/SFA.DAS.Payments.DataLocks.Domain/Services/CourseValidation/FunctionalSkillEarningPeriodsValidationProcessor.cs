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
            int academicYear);
    }

    public class FunctionalSkillEarningPeriodsValidationProcessor : BaseEarningPeriodsValidationProcessor , IFunctionalSkillEarningPeriodsValidationProcessor
    {
        private readonly IFunctionalSkillValidationProcessor functionalSkillValidationProcessor;

        public FunctionalSkillEarningPeriodsValidationProcessor(IFunctionalSkillValidationProcessor functionalSkillValidationProcessor)
        {
            this.functionalSkillValidationProcessor = functionalSkillValidationProcessor;
        }

        protected override (List<ApprenticeshipModel> validApprenticeships, List<DataLockFailure> dataLockFailures) ValidateApprenticeships(
            long ukprn, 
            List<ApprenticeshipModel> apprenticeships,
            int academicYear, 
            EarningPeriod period, 
            TransactionType transactionType,
            List<PriceEpisode> priceEpisodes)
        {
            return (apprenticeships, new List<DataLockFailure>());
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
            int academicYear)
        {
            return base.ValidateEarningPeriods(ukprn, uln, periods, transactionType, apprenticeships, aim, academicYear);
        }
    }
}