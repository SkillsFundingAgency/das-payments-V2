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

        public async Task<DataLockEvent> Validate(ApprenticeshipContractType1EarningEvent earningEvent, CancellationToken cancellationToken)
        {
            var learnerMatchResult = await learnerMatcher.MatchLearner(earningEvent.Learner.Uln).ConfigureAwait(false);
            if (learnerMatchResult.DataLockErrorCode.HasValue) return CreateDataLockNonPayableEarningEvent(earningEvent, learnerMatchResult.DataLockErrorCode.Value);

            var apprenticeshipsForUln = learnerMatchResult.Apprenticeships;

            foreach (var onProgrammeEarning in earningEvent.OnProgrammeEarnings)
            {
                var validOnProgrammeEarningPeriods = ValidOnProgEarningPeriods(earningEvent.Learner.Uln, earningEvent.PriceEpisodes, onProgrammeEarning, apprenticeshipsForUln);
                onProgrammeEarning.Periods = new ReadOnlyCollection<EarningPeriod>(validOnProgrammeEarningPeriods);
            }

            var returnMessage = mapper.Map<PayableEarningEvent>(earningEvent);
            var apprenticeship = apprenticeshipsForUln.FirstOrDefault();

            returnMessage.AccountId = apprenticeship.AccountId;
            returnMessage.Priority = apprenticeship.Priority;

            return returnMessage;
        }

        private NonPayableEarningEvent CreateDataLockNonPayableEarningEvent( ApprenticeshipContractType1EarningEvent earningEvent, DataLockErrorCode dataLockErrorCode)
        {
            var nonPayableEarning = mapper.Map<NonPayableEarningEvent>(earningEvent);
            nonPayableEarning.Errors = new ReadOnlyCollection<DataLockErrorCode>(
                new[]
                {
                    dataLockErrorCode
                });
            return nonPayableEarning;
        }

        private List<EarningPeriod> ValidOnProgEarningPeriods(long uln, List<PriceEpisode> priceEpisodes, OnProgrammeEarning onProgrammeEarning, List<ApprenticeshipModel> apprenticeships)
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

            return validOnProgrammeEarningPeriods;
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
