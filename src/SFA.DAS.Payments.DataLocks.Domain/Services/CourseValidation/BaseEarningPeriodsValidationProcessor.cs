using System.Collections.Generic;
using System.Linq;
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
             List<EarningPeriod> periods,
             TransactionType transactionType,
             List<ApprenticeshipModel> apprenticeships,
             LearningAim aim,
             int academicYear,
             List<PriceEpisode> priceEpisodes = null)
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

                var newEarningPeriod = CreateEarningPeriod(period);

                var initialValidationResult = ValidateApprenticeships(ukprn, apprenticeships, academicYear, period, transactionType, priceEpisodes);
                if (initialValidationResult.dataLockFailures.Any())
                {
                    var apprenticeshipsWithDataLock = apprenticeships
                        .Where(a => initialValidationResult.dataLockFailures.Any(d => d.ApprenticeshipId == a.Id))
                        .ToList();

                    newEarningPeriod.DataLockFailures = GetLatestApprenticeshipDataLocks(initialValidationResult.dataLockFailures, apprenticeshipsWithDataLock);
                    invalidPeriods.Add(newEarningPeriod);
                    continue;
                }

                var validationResults = initialValidationResult.validApprenticeships
                    .Select(a => ValidateApprenticeship(uln, transactionType, aim, academicYear, period, a, priceEpisodes))
                    .ToList();

                var matchedApprenticeships = initialValidationResult.validApprenticeships
                    .Where(a => validationResults.Any(x => x.MatchedPriceEpisode != null && x.MatchedPriceEpisode.ApprenticeshipId == a.Id))
                    .ToList();

                var apprenticeshipsWithError = initialValidationResult.validApprenticeships
                    .Where(a => validationResults.Any(x => x.DataLockFailures.Any(d => d.ApprenticeshipId == a.Id)))
                    .ToList();

                var validApprenticeshipIds = matchedApprenticeships.Select(x => x.Id).Except(apprenticeshipsWithError.Select(o => o.Id))
                    .ToList();

                if (!validApprenticeshipIds.Any())
                {
                    var allPeriodDataLockFailures = validationResults.SelectMany(x => x.DataLockFailures).ToList();
                    newEarningPeriod.DataLockFailures = GetLatestApprenticeshipDataLocks(allPeriodDataLockFailures, apprenticeshipsWithError);
                    invalidPeriods.Add(newEarningPeriod);
                    continue;
                }

                var validApprenticeshipId = validApprenticeshipIds.Max(); //todo trigger DLOCK_08 if more than one apprenticeship
                var apprenticeship = initialValidationResult.validApprenticeships.First(x => x.Id == validApprenticeshipId);
                var apprenticeCourseValidationResult = validationResults.First(v => v.MatchedPriceEpisode != null && v.MatchedPriceEpisode.ApprenticeshipId == validApprenticeshipId);
                var earningPeriodWithApprenticeshipRecord = BuildEarningPeriodWithApprenticeship(period, apprenticeship, apprenticeCourseValidationResult);

                validPeriods.Add(earningPeriodWithApprenticeshipRecord);
            }

            return (validPeriods, invalidPeriods);
        }


        private EarningPeriod BuildEarningPeriodWithApprenticeship(EarningPeriod period, ApprenticeshipModel apprenticeship, CourseValidationResult validationResult)
        {
            var newEarningPeriod = CreateEarningPeriod(period);
            newEarningPeriod.AccountId = apprenticeship.AccountId;
            newEarningPeriod.ApprenticeshipId = apprenticeship.Id;
            newEarningPeriod.ApprenticeshipPriceEpisodeId = validationResult.MatchedPriceEpisode.Id;
            newEarningPeriod.TransferSenderAccountId = apprenticeship.TransferSendingEmployerAccountId;
            newEarningPeriod.Priority = apprenticeship.Priority;
            newEarningPeriod.AgreedOnDate = apprenticeship.AgreedOnDate;
            newEarningPeriod.ApprenticeshipEmployerType = apprenticeship.ApprenticeshipEmployerType;

            return newEarningPeriod;
        }


        private List<DataLockFailure> GetLatestApprenticeshipDataLocks(List<DataLockFailure> allDataLockFailures, List<ApprenticeshipModel> apprenticeships)
        {
            var maxStartDate = apprenticeships.Max(x => x.EstimatedStartDate);
            var latestApprenticeshipId =  apprenticeships
                .Where(x => x.EstimatedStartDate == maxStartDate)
                .Max(x => x.Id);

            return allDataLockFailures.Where(x => x.ApprenticeshipId == latestApprenticeshipId).ToList();
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
}