using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public class OnProgrammePeriodsValidationProcessor : IOnProgrammePeriodsValidationProcessor
    {
        private readonly ICourseValidationProcessor courseValidationProcessor;

        public OnProgrammePeriodsValidationProcessor(ICourseValidationProcessor courseValidationProcessor)
        {
            this.courseValidationProcessor = courseValidationProcessor ?? throw new ArgumentNullException(nameof(courseValidationProcessor));
        }

        public (List<EarningPeriod> ValidPeriods, List<EarningPeriod> InValidPeriods) ValidatePeriods(long uln,
            List<PriceEpisode> priceEpisodes, OnProgrammeEarning onProgrammeEarning,
            List<ApprenticeshipModel> apprenticeships, LearningAim aim)
        {
            var validPeriods = new List<EarningPeriod>();
            var invalidPeriods = new List<EarningPeriod>();
            foreach (var period in onProgrammeEarning.Periods)
            {
                if (period.Amount == decimal.Zero)
                {
                    validPeriods.Add(period);
                    continue;
                }

                foreach (var apprenticeship in apprenticeships)
                {
                    var validationModel = new DataLockValidationModel
                    {
                        Uln = uln,
                        EarningPeriod = period,
                        Apprenticeship = apprenticeship,
                        PriceEpisode = priceEpisodes.SingleOrDefault(o => o.Identifier.Equals(period.PriceEpisodeIdentifier, StringComparison.OrdinalIgnoreCase))
                                       ?? throw new InvalidOperationException($"Failed to find price episode: {period.PriceEpisodeIdentifier} for uln: {uln}, earning: {onProgrammeEarning.Type:G}, period: {period.Period}"),
                        Aim = aim,
                    };

                    var validationResult = courseValidationProcessor.ValidateCourse(validationModel);

                    var newEarningPeriod = CreateEarningPeriod(period);
                    if (validationResult.DataLockFailures.Any())
                    {
                        newEarningPeriod.DataLockFailures = validationResult.DataLockFailures;
                        invalidPeriods.Add(newEarningPeriod);
                    }
                    else
                    {
                        newEarningPeriod.AccountId = apprenticeship.AccountId;
                        newEarningPeriod.ApprenticeshipId = apprenticeship.Id;
                        newEarningPeriod.ApprenticeshipPriceEpisodeId = validationResult.MatchedPriceEpisode.Id;
                        newEarningPeriod.Priority = apprenticeship.Priority;
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