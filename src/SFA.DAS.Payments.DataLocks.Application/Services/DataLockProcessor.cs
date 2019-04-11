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
           var validationResults = new List<ValidationResult>();
            
            var learnerMatchResult = await learnerMatcher.MatchLearner(earningEvent.Learner.Uln);

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
          

            foreach (var onProgrammeEarning in earningEvent.OnProgrammeEarnings)
            {
                var onProgrammeEarningPeriods = onProgrammeEarning.Periods;
                var validOnProgrammeEarningPeriods = new List<EarningPeriod>();

                foreach (var period  in onProgrammeEarningPeriods)
                {
                    var validationModel = CreateDataLockValidationModel(earningEvent.Learner.Uln ,earningEvent.PriceEpisodes, period, apprenticeshipsForUln);
                    var periodValidationResults =  processCourseValidator.ValidateCourse(validationModel);

                    if (periodValidationResults != null && periodValidationResults.Any())
                    {
                        validationResults.AddRange(periodValidationResults);

                    }
                    else
                    {
                        validOnProgrammeEarningPeriods.Add(period);
                    }
                }

                onProgrammeEarning.Periods = new ReadOnlyCollection<EarningPeriod>(validOnProgrammeEarningPeriods);
            }

            var returnMessage = mapper.Map<PayableEarningEvent>(earningEvent);
            var apprenticeship = apprenticeshipsForUln.FirstOrDefault();
            
            returnMessage.AccountId = apprenticeship.AccountId;
            returnMessage.Priority = apprenticeship.Priority;

            return returnMessage;
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
