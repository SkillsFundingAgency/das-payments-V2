using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation
{
    public interface IOnProgrammePeriodsValidationProcessor
    {
        (List<ValidOnProgrammePeriod> ValidPeriods, List<InvalidOnProgrammePeriod> InValidPeriods) ValidatePeriods(
            long uln, List<PriceEpisode> priceEpisodes, OnProgrammeEarning onProgrammeEarning,
            List<ApprenticeshipModel> apprenticeships);
    }


    public class OnProgrammePeriodsValidationProcessor: IOnProgrammePeriodsValidationProcessor
    {
        private readonly ICourseValidationProcessor courseValidationProcessor;

        public OnProgrammePeriodsValidationProcessor(ICourseValidationProcessor courseValidationProcessor)
        {
            this.courseValidationProcessor = courseValidationProcessor ?? throw new ArgumentNullException(nameof(courseValidationProcessor));
        }

        public (List<ValidOnProgrammePeriod> ValidPeriods, List<InvalidOnProgrammePeriod> InValidPeriods) ValidatePeriods(long uln, List<PriceEpisode> priceEpisodes, OnProgrammeEarning onProgrammeEarning, List<ApprenticeshipModel> apprenticeships)
        {
            var validPeriods = new List<ValidOnProgrammePeriod>();
            var invalidPeriods = new List<InvalidOnProgrammePeriod>();
            foreach (var period in onProgrammeEarning.Periods)
            {
                if (period.Amount == decimal.Zero)
                {
                    
                    validPeriods.Add(new ValidOnProgrammePeriod
                    {
                        Period = period
                    });
                    continue;
                }

                foreach (var apprenticeship in apprenticeships)
                {
                    var validationModel = new DataLockValidationModel
                    {
                        Uln = uln,
                        EarningPeriod = period,
                        ApprenticeshipId = apprenticeship.Id,
                        PriceEpisode = priceEpisodes.SingleOrDefault(o => o.Identifier.Equals(period.PriceEpisodeIdentifier, StringComparison.OrdinalIgnoreCase))
                                       ?? throw new InvalidOperationException($"Failed to find price episode: {period.PriceEpisodeIdentifier} for uln: {uln}, earning: {onProgrammeEarning.Type:G}, period: {period.Period}")
                    };

                    var validationResult = courseValidationProcessor.ValidateCourse(validationModel);
                    if (validationResult.DataLockErrors.Any())
                        invalidPeriods.Add(new InvalidOnProgrammePeriod
                        {
                            DataLockErrors = validationResult.DataLockErrors,
                            Apprenticeship = apprenticeship,
                            Period = period
                        });
                    else
                        validPeriods.Add(new ValidOnProgrammePeriod
                        {
                            ApprenticeshipPriceEpisodeId = validationResult.MatchedPriceEpisode.Id,
                            Apprenticeship = apprenticeship,
                            Period = period
                        });
                }
            }
            return (validPeriods, invalidPeriods);
        }
    }
}