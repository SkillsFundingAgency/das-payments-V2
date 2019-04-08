using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

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

            var learnerMatchResult = await learnerMatcher.MatchLearner(courseValidation);

            if (learnerMatchResult.DataLockErrorCode.HasValue)
            {
                // TODO: return non-payable earning
                return null;
            }

            var apprenticeshipsForUln = learnerMatchResult.Apprenticeships;

            var courseValidationResult = await courseValidator.ValidateCourse(courseValidation, apprenticeshipsForUln);

            if (courseValidationResult.ValidationResults.Any(x => x.DataLockErrorCode.HasValue))
            {
                // TODO: return non-payable earning
                return null;
            }

            var apprenticeship = apprenticeshipsForUln.FirstOrDefault();

            var returnMessage = mapper.Map<PayableEarningEvent>(earningEvent);

            returnMessage.AccountId = apprenticeship.AccountId;
            returnMessage.Priority = apprenticeship.Priority;

            return returnMessage;
        }
    }
}
