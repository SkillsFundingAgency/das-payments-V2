using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public abstract class BaseEarningPeriodsValidationProcessor
    {

        protected abstract (List<ApprenticeshipModel> validApprenticeships, List<DataLockFailure> dataLockFailures)
            ValidateApprenticeships
            (long ukprn,
                List<ApprenticeshipModel> apprenticeships,
                int academicYear,
                EarningPeriod period,
                TransactionType transactionType1,
                List<PriceEpisode> priceEpisodes);

        protected abstract CourseValidationResult ValidateApprenticeship(
            long uln,
            TransactionType transactionType,
            LearningAim aim,
            int academicYear,
            EarningPeriod period,
            ApprenticeshipModel apprenticeship,
            List<PriceEpisode> priceEpisodes = null);

        protected (List<EarningPeriod> ValidPeriods, List<EarningPeriod> InValidPeriods) ValidateEarningPeriods
         (
             long ukprn,
             long uln,
             List<PriceEpisode> priceEpisodes,
             List<EarningPeriod> periods,
             TransactionType transactionType,
             List<ApprenticeshipModel> apprenticeships,
             LearningAim aim,
             int academicYear)
        {
            var validPeriods = new List<EarningPeriod>();
            var invalidPeriods = new List<EarningPeriod>();

            foreach (var period in periods)
            {
                if (period.Amount == decimal.Zero)
                {
                    validPeriods.Add(period);
                    continue;
                }

                var initialValidationResult = ValidateApprenticeships(ukprn, apprenticeships, academicYear, period,transactionType, priceEpisodes);
                if (initialValidationResult.dataLockFailures.Any())
                {
                    var newEarningPeriod = CreateEarningPeriod(period);
                    newEarningPeriod.DataLockFailures = initialValidationResult.dataLockFailures;
                    invalidPeriods.Add(newEarningPeriod);

                    continue;
                }

                foreach (var apprenticeship in initialValidationResult.validApprenticeships)
                {
                    var validationResult = ValidateApprenticeship(uln, transactionType, aim, academicYear, period, apprenticeship, priceEpisodes);

                    var newEarningPeriod = CreateEarningPeriod(period);
                    if (validationResult.DataLockFailures.Any())
                    {
                        var invalidPeriod = invalidPeriods.FirstOrDefault(x => x.Period == period.Period);

                        if (invalidPeriod == null)
                        {
                            newEarningPeriod.DataLockFailures = validationResult.DataLockFailures;
                            invalidPeriods.Add(newEarningPeriod);
                        }
                        else
                        {
                            invalidPeriod.DataLockFailures.AddRange(validationResult.DataLockFailures);
                        }
                    }
                    else
                    {
                        newEarningPeriod.AccountId = apprenticeship.AccountId;
                        newEarningPeriod.ApprenticeshipId = apprenticeship.Id;
                        newEarningPeriod.ApprenticeshipPriceEpisodeId = validationResult.MatchedPriceEpisode.Id;
                        newEarningPeriod.TransferSenderAccountId = apprenticeship.TransferSendingEmployerAccountId;
                        newEarningPeriod.Priority = apprenticeship.Priority;
                        newEarningPeriod.AgreedOnDate = apprenticeship.AgreedOnDate;
                        newEarningPeriod.ApprenticeshipEmployerType = apprenticeship.ApprenticeshipEmployerType;
                        validPeriods.Add(newEarningPeriod);
                    }
                }
            }

            return (validPeriods, invalidPeriods);
        }

        private EarningPeriod CreateEarningPeriod(EarningPeriod period)
        {
            return new EarningPeriod
            {
                Period = period.Period,
                Amount = period.Amount,
                PriceEpisodeIdentifier = period.PriceEpisodeIdentifier,
                SfaContributionPercentage = period.SfaContributionPercentage
            };
        }

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
            return base.ValidateEarningPeriods(ukprn, uln, priceEpisodes, periods, transactionType, apprenticeships, aim, academicYear);
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