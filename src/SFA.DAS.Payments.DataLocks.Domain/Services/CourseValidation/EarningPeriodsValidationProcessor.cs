using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class EarningPeriodsValidationProcessor : IEarningPeriodsValidationProcessor
    {
        private readonly ICourseValidationProcessor courseValidationProcessor;

        public EarningPeriodsValidationProcessor(ICourseValidationProcessor courseValidationProcessor)
        {
            this.courseValidationProcessor = courseValidationProcessor ?? throw new ArgumentNullException(nameof(courseValidationProcessor));
        }

        public (List<EarningPeriod> ValidPeriods, List<EarningPeriod> InValidPeriods) ValidatePeriods(long ukprn,
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

                foreach (var apprenticeship in apprenticeships.Where(apprenticeship => apprenticeship.Ukprn == ukprn))
                {
                    var validationModel = new DataLockValidationModel
                    {
                        EarningPeriod = period,
                        Apprenticeship = apprenticeship,
                        PriceEpisode = priceEpisodes.SingleOrDefault(o => o.Identifier.Equals(period.PriceEpisodeIdentifier, StringComparison.OrdinalIgnoreCase))
                                       ?? throw new InvalidOperationException($"Failed to find price episode: {period.PriceEpisodeIdentifier} for uln: {uln}, earning: {transactionType:G}, period: {period.Period}"),
                        TransactionType = transactionType,
                        Aim = aim,
                        AcademicYear = academicYear
                    };

                    var validationResult = courseValidationProcessor.ValidateCourse(validationModel);
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
}