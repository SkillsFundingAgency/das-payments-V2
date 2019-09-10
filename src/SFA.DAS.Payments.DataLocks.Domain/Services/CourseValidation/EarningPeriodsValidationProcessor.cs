﻿using System;
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
        private readonly IFunctionalSkillValidationProcessor functionalSkillValidationProcessor;
       
        public EarningPeriodsValidationProcessor(ICourseValidationProcessor courseValidationProcessor, 
            IFunctionalSkillValidationProcessor functionalSkillValidationProcessor)
        {
            this.courseValidationProcessor = courseValidationProcessor ?? throw new ArgumentNullException(nameof(courseValidationProcessor));
            this.functionalSkillValidationProcessor = functionalSkillValidationProcessor?? throw new ArgumentNullException(nameof(functionalSkillValidationProcessor));
        }

        public (List<EarningPeriod> ValidPeriods, List<EarningPeriod> InValidPeriods) ValidateFunctionalSkillPeriods(
            long ukprn,
            long uln,
            List<PriceEpisode> priceEpisodes,
            List<EarningPeriod> periods,
            TransactionType transactionType,
            List<ApprenticeshipModel> apprenticeships,
            LearningAim aim,
            int academicYear)
        {
            return ValidateEarningPeriods(ukprn, uln, priceEpisodes, periods, transactionType, apprenticeships, aim, academicYear, functionalSkillValidationProcessor);
        }

        public (List<EarningPeriod> ValidPeriods, List<EarningPeriod> InValidPeriods) ValidatePeriods(
            long ukprn,
            long uln,
            List<PriceEpisode> priceEpisodes,
            List<EarningPeriod> periods,
            TransactionType transactionType,
            List<ApprenticeshipModel> apprenticeships,
            LearningAim aim,
            int academicYear)
        {
            return ValidateEarningPeriods(ukprn, uln, priceEpisodes, periods, transactionType, apprenticeships, aim, academicYear, courseValidationProcessor);
        }

        private (List<EarningPeriod> ValidPeriods, List<EarningPeriod> InValidPeriods) ValidateEarningPeriods(
            long ukprn,
            long uln,
            List<PriceEpisode> priceEpisodes,
            List<EarningPeriod> periods,
            TransactionType transactionType,
            List<ApprenticeshipModel> apprenticeships,
            LearningAim aim,
            int academicYear, ICourseValidationProcessor processor)
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
                
                foreach (var apprenticeship in apprenticeships)
                {
                    var validationModel = new DataLockValidationModel
                    {
                        EarningPeriod = period,
                        Apprenticeship = apprenticeship,
                        PriceEpisode = IsFunctionalSkillTransactionType(transactionType) 
                            ? null
                            : priceEpisodes.SingleOrDefault(o =>
                                  o.Identifier.Equals(period.PriceEpisodeIdentifier, StringComparison.OrdinalIgnoreCase))
                                 ?? throw new InvalidOperationException($"Failed to find price episode: {period.PriceEpisodeIdentifier} for uln: {uln}, earning: {transactionType:G}, period: {period.Period}"),
                        TransactionType = transactionType,
                        Aim = aim,
                        AcademicYear = academicYear
                    };

                    var validationResult = processor.ValidateCourse(validationModel);
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

        private bool IsFunctionalSkillTransactionType(TransactionType transactionType)
        {
            var functionalSkillTransactionTypes = new List<TransactionType>
                {TransactionType.OnProgrammeMathsAndEnglish, TransactionType.BalancingMathsAndEnglish};

            return functionalSkillTransactionTypes.Contains(transactionType);
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