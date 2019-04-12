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

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class DataLockProcessor : IDataLockProcessor
    {
        private readonly IMapper mapper;
        private readonly ILearnerMatcher learnerMatcher;
        private readonly ICourseValidator courseValidator;

        public DataLockProcessor(IMapper mapper, ILearnerMatcher learnerMatcher, ICourseValidator courseValidator)
        {
            this.mapper = mapper;
            this.learnerMatcher = learnerMatcher;
            this.courseValidator = courseValidator;
        }

        public async Task<DataLockEvent> Validate(ApprenticeshipContractType1EarningEvent earningEvent,
            CancellationToken cancellationToken)
        {
            var startDate = earningEvent.PriceEpisodes.FirstOrDefault()?.StartDate ?? DateTime.UtcNow;

         
            var courseValidation = new DataLockValidation
            {
                Uln = earningEvent.Learner.Uln,
                StartDate = startDate
            };

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

            var courseValidationResult = await courseValidator.ValidateCourse(courseValidation, apprenticeshipsForUln);

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
    }
}
