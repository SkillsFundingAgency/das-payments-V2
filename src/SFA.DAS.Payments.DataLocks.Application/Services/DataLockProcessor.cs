using AutoMapper;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class DataLockProcessor : IDataLockProcessor
    {
        private readonly IMapper mapper;
        private readonly ILearnerMatcher learnerMatcher;
        private readonly IProcessCourseValidator processCourseValidator;

        public DataLockProcessor(IMapper mapper, ILearnerMatcher learnerMatcher, IProcessCourseValidator processCourseValidator)
        {
            this.mapper = mapper;
            this.learnerMatcher = learnerMatcher;
            this.processCourseValidator = processCourseValidator;
        }

        public async Task<List<DataLockEvent>> Validate(ApprenticeshipContractType1EarningEvent earningEvent,
            CancellationToken cancellationToken)
        {
            var learnerMatchResult = await learnerMatcher.MatchLearner(earningEvent.Learner.Uln).ConfigureAwait(false);
            if (learnerMatchResult.DataLockErrorCode.HasValue)
                return new List<DataLockEvent>
                {
                    CreateNonPayableEarningEvent(earningEvent, learnerMatchResult.DataLockErrorCode.Value)
                };

            var apprenticeshipsForUln = learnerMatchResult.Apprenticeships;

            var result = new List<DataLockEvent>();

            foreach (var onProgrammeEarning in earningEvent.OnProgrammeEarnings)
            {
                var onProgrammeEarningPeriods = ValidateOnProgEarningPeriods(earningEvent.Learner.Uln, earningEvent.PriceEpisodes, onProgrammeEarning, apprenticeshipsForUln);

                var validPeriods = onProgrammeEarningPeriods.ValidPeriods;
                var validationErrors = onProgrammeEarningPeriods.ValidationErrors;

                if (validationErrors != null && validationErrors.Any())
                {
                    result.Add(CreateNonPayableEarningEvent(earningEvent, validationErrors));
                }

                onProgrammeEarning.Periods = new ReadOnlyCollection<EarningPeriod>(validPeriods);
            }

            result.Add(CreatePayableEarningEvent(earningEvent, apprenticeshipsForUln));

            return result;
        }

        private DataLockEvent CreatePayableEarningEvent(ApprenticeshipContractType1EarningEvent earningEvent,
             List<ApprenticeshipModel> apprenticeshipsForUln)
        {
            var result = mapper.Map<PayableEarningEvent>(earningEvent);
            var apprenticeship = apprenticeshipsForUln.FirstOrDefault();

            result.AccountId = apprenticeship.AccountId;
            result.Priority = apprenticeship.Priority;

            return result;
        }

        private DataLockEvent CreateNonPayableEarningEvent(ApprenticeshipContractType1EarningEvent earningEvent,
            List<ValidationResult> validationResults)
        {
            var result = mapper.Map<NonPayableEarningEvent>(earningEvent);

            result.Errors = new ReadOnlyCollection<DataLockErrorCode>(
                validationResults.Select(x => x.DataLockErrorCode).Distinct().ToList());

            return result;
        }

        private NonPayableEarningEvent CreateNonPayableEarningEvent(ApprenticeshipContractType1EarningEvent earningEvent, DataLockErrorCode dataLockErrorCode)
        {
            var nonPayableEarning = mapper.Map<NonPayableEarningEvent>(earningEvent);
            nonPayableEarning.Errors = new ReadOnlyCollection<DataLockErrorCode>(
                new[]
                {
                    dataLockErrorCode
                });
            return nonPayableEarning;
        }

        private (List<EarningPeriod> ValidPeriods, List<ValidationResult> ValidationErrors) ValidateOnProgEarningPeriods(long uln, List<PriceEpisode> priceEpisodes, OnProgrammeEarning onProgrammeEarning, List<ApprenticeshipModel> apprenticeships)
        {
            var onProgrammeEarningPeriods = onProgrammeEarning.Periods;
            var validOnProgrammeEarningPeriods = new List<EarningPeriod>();
            var validationResults = new List<ValidationResult>();

            foreach (var period in onProgrammeEarningPeriods)
            {
                var validationModel = CreateDataLockValidationModel(uln, priceEpisodes, period, apprenticeships);
                var periodValidationResults = processCourseValidator.ValidateCourse(validationModel);

                if (periodValidationResults != null && periodValidationResults.Any())
                {
                    validationResults.AddRange(periodValidationResults);
                }
                else
                {
                    validOnProgrammeEarningPeriods.Add(period);
                }
            }

            return (validOnProgrammeEarningPeriods, validationResults);
        }

        private DataLockValidation CreateDataLockValidationModel(long uln, List<PriceEpisode> priceEpisodes, EarningPeriod earningPeriod, List<ApprenticeshipModel> apprenticeships)
        {
            return new DataLockValidation
            {
                Uln = uln,
                EarningPeriod = earningPeriod,
                Apprenticeships = apprenticeships,
                PriceEpisode = priceEpisodes.Single(o => o.Identifier.Equals(earningPeriod.PriceEpisodeIdentifier, StringComparison.OrdinalIgnoreCase))
            };
        }


    }
}
