using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

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
            var startDate = earningEvent.PriceEpisodes.FirstOrDefault()?.StartDate ?? DateTime.UtcNow;

           var validationResults = new List<ValidationResult>();
            
            var learnerMatchResult = await learnerMatcher.MatchLearner(courseValidation);

            if (learnerMatchResult.DataLockErrorCode.HasValue)
            {
                var nonPayableEarning = mapper.Map<NonPayableEarningEvent>(earningEvent);
                nonPayableEarning.Errors = new ReadOnlyCollection<DataLockErrorCode>(
                    new[]
                    {
                        learnerMatchResult.DataLockErrorCode.Value
                    });

                return nonPayableEarning;
            }

            var apprenticeshipsForUln = learnerMatchResult.Apprenticeships;
            var validOnProgrammeEarningPeriods = new List<EarningPeriod>();

            foreach (var onProgrammeEarning in earningEvent.OnProgrammeEarnings)
            {
                var onProgrammeEarningPeriods = onProgrammeEarning.Periods;
               
                
                foreach (var period  in onProgrammeEarningPeriods)
                {
                    var validationModel = CreateDataLockValidationModel(earningEvent.Learner.Uln ,earningEvent.PriceEpisodes, period, apprenticeshipsForUln);
                    var validationResult =  processCourseValidator.ValidateCourse(validationModel);

                    if (validationResult != null && validationResults.Any())
                    {
                        validationResults.AddRange(validationResult);
                    }
                }

                onProgrammeEarning.Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>());


            }

            var returnMessage = mapper.Map<PayableEarningEvent>(earningEvent);

            if (courseValidationResult.Any(x => x.DataLockErrorCode.HasValue))
            {
                ProcessDataLockErrors(courseValidationResult, returnMessage);
            }

            var apprenticeship = apprenticeshipsForUln.FirstOrDefault();
            
            returnMessage.AccountId = apprenticeship.AccountId;
            returnMessage.Priority = apprenticeship.Priority;

            return returnMessage;
        }

        private void ProcessDataLockErrors(List<ValidationResult> courseValidation, PayableEarningEvent returnMessage)
        {
            foreach (var result in courseValidation)
            {
                foreach (var onProgrammeEarning in returnMessage.OnProgrammeEarnings)
                {
                    var matchingPeriods = onProgrammeEarning.Periods.Where(x =>
                        x.Period == result.Period &&
                        x.PriceEpisodeIdentifier == result.ApprenticeshipPriceEpisodeIdentifier);

                    onProgrammeEarning.Periods = new ReadOnlyCollection<EarningPeriod>(onProgrammeEarning.Periods.Except(matchingPeriods).ToList());
                }
            }
        }

        private DataLockValidation CreateDataLockValidationModel(long uln, 
            List<PriceEpisode> priceEpisodes, 
            EarningPeriod earningPeriod, 
            List<ApprenticeshipModel> apprenticeships)
        {
            return new DataLockValidation
            {
                Uln = uln,
                EarningPeriod = earningPeriod,
                Apprenticeships = apprenticeships,
                PriceEpisode = priceEpisodes.Single(o => o.Identifier.Equals(earningPeriod.PriceEpisodeIdentifier,StringComparison.OrdinalIgnoreCase))
            };
        }


    }
}
