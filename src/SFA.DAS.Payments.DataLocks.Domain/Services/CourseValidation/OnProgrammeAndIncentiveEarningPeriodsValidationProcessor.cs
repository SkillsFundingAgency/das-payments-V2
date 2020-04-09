using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public interface IOnProgrammeAndIncentiveEarningPeriodsValidationProcessor
    {
        (List<EarningPeriod> ValidPeriods, List<EarningPeriod> InValidPeriods) ValidatePeriods(
            long ukprn,
            long uln,
            List<PriceEpisode> priceEpisodes,
            List<EarningPeriod> periods,
            TransactionType transactionType,
            List<ApprenticeshipModel> apprenticeships,
            LearningAim aim,
            int academicYear);
    }

    public class OnProgrammeAndIncentiveEarningPeriodsValidationProcessor : BaseEarningPeriodsValidationProcessor, IOnProgrammeAndIncentiveEarningPeriodsValidationProcessor
    {
        private readonly IStartDateValidator startDateValidator;
        private readonly IOnProgrammeAndIncentiveStoppedValidator onProgrammeAndIncentiveStoppedValidator;
        private readonly ICompletionStoppedValidator completionStoppedValidator;
        private readonly ICourseValidationProcessor courseValidationProcessor;

        public OnProgrammeAndIncentiveEarningPeriodsValidationProcessor(
            IStartDateValidator startDateValidator,
            IOnProgrammeAndIncentiveStoppedValidator onProgrammeAndIncentiveStoppedValidator,
            ICompletionStoppedValidator completionStoppedValidator,
            ICourseValidationProcessor courseValidationProcessor)
        {
            this.startDateValidator = startDateValidator;
            this.onProgrammeAndIncentiveStoppedValidator = onProgrammeAndIncentiveStoppedValidator;
            this.completionStoppedValidator = completionStoppedValidator;
            this.courseValidationProcessor = courseValidationProcessor;
        }

        public (List<EarningPeriod> ValidPeriods, List<EarningPeriod> InValidPeriods) ValidatePeriods(long ukprn, long uln, List<PriceEpisode> priceEpisodes, List<EarningPeriod> periods, TransactionType transactionType, List<ApprenticeshipModel> apprenticeships, LearningAim aim, int academicYear)
        {
            return base.ValidateEarningPeriods(ukprn, uln, periods, transactionType, apprenticeships, aim, academicYear, priceEpisodes);
        }

        protected override (List<ApprenticeshipModel> validApprenticeships, List<DataLockFailure> dataLockFailures) ValidateApprenticeships(long ukprn, List<ApprenticeshipModel> apprenticeships, int academicYear, EarningPeriod period,TransactionType transactionType, List<PriceEpisode> priceEpisodes)
        {
            var ilrPriceEpisode = priceEpisodes.SingleOrDefault(o => o.Identifier.Equals(period.PriceEpisodeIdentifier, StringComparison.OrdinalIgnoreCase))
                                  ?? throw new InvalidOperationException($"Failed to find price episode: {period.PriceEpisodeIdentifier} for ukprn: {ukprn}, earning: {transactionType:G}, period: {period.Period}");

            var startDateValidationResult = startDateValidator.Validate(ilrPriceEpisode, apprenticeships);
            if (startDateValidationResult.dataLockFailures.Any())
            {
                return (new List<ApprenticeshipModel>(), startDateValidationResult.dataLockFailures);
            }
            
            var onProgrammeValidationResult = onProgrammeAndIncentiveStoppedValidator.Validate(startDateValidationResult.validApprenticeships, transactionType,period,academicYear);
            if (onProgrammeValidationResult.dataLockFailures.Any())
            {
                return (new List<ApprenticeshipModel>(), onProgrammeValidationResult.dataLockFailures);
            }

            var completionStoppedResult = completionStoppedValidator.Validate(ilrPriceEpisode, onProgrammeValidationResult.validApprenticeships, transactionType);
            if (completionStoppedResult.dataLockFailures.Any())
            {
                return (new List<ApprenticeshipModel>(), completionStoppedResult.dataLockFailures);
            }

            return (completionStoppedResult.validApprenticeships, new List<DataLockFailure>());
        }

        protected override CourseValidationResult ValidateApprenticeship(long uln, TransactionType transactionType, LearningAim aim, int academicYear, EarningPeriod period, ApprenticeshipModel apprenticeship, List<PriceEpisode> priceEpisodes = null)
        {
            var validationModel = new DataLockValidationModel
            {
                EarningPeriod = period,
                Apprenticeship = apprenticeship,
                PriceEpisode = priceEpisodes.SingleOrDefault(o =>
                                   o.Identifier.Equals(period.PriceEpisodeIdentifier, StringComparison.OrdinalIgnoreCase))
                               ?? throw new InvalidOperationException($"Failed to find price episode: {period.PriceEpisodeIdentifier} for uln: {uln}, earning: {transactionType:G}, period: {period.Period}"),
                TransactionType = transactionType,
                Aim = aim,
                AcademicYear = academicYear
            };
            
            return courseValidationProcessor.ValidateCourse(validationModel);
        }

    }
}