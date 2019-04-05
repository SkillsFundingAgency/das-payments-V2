using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.DataLockService.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.DataLockService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class DataLockService : Actor, IDataLockService
    {
        private readonly ActorService actorService;
        private readonly ActorId actorId;
        private readonly IMapper mapper;
        private readonly IPaymentLogger paymentLogger;
        private readonly IActorDataCache<List<ApprenticeshipModel>> apprenticeships;
        private readonly ILearnerMatcher learnerMatcher;
        private readonly ICourseValidator courseValidator;
        private readonly IApprenticeshipRepository apprenticeshipRepository;

        public DataLockService(
            ActorService actorService, 
            ActorId actorId, 
            IMapper mapper,
            IPaymentLogger paymentLogger, 
            IApprenticeshipRepository apprenticeshipRepository,
            IActorDataCache<List<ApprenticeshipModel>> apprenticeships,
            ILearnerMatcher learnerMatcher,
            ICourseValidator courseValidator) 
            : base(actorService, actorId)
        {
            this.actorService = actorService;
            this.actorId = actorId;
            this.mapper = mapper;
            this.paymentLogger = paymentLogger;
            this.apprenticeshipRepository = apprenticeshipRepository;
            this.apprenticeships = apprenticeships;
            this.learnerMatcher = learnerMatcher;
            this.courseValidator = courseValidator;
        }

        public async Task<DataLockEvent> HandleEarning(ApprenticeshipContractType1EarningEvent message, CancellationToken cancellationToken)
        {
            var learnerMatchResult = await learnerMatcher.MatchLearner(message.Ukprn, message.Ukprn);

            if (learnerMatchResult.DataLockErrorCode.HasValue)
            {
                // TODO: return non-payable earning
                return null;
            }

            var apprenticeshipsForUln = learnerMatchResult.Apprenticeships;
            var apprenticeship = apprenticeshipsForUln.FirstOrDefault();

            var returnMessage = mapper.Map<PayableEarningEvent>(message);

            // TODO: After implementing CourseValidator, need correct message going into CourseValidator
            var courseValidationResult = await courseValidator.ValidateCourse(returnMessage.CollectionPeriod, apprenticeshipsForUln);

            if (courseValidationResult.ValidationResults.Any(x => x.DataLockErrorCode.HasValue))
            {
                // TODO: return non-payable earning
                return null;
            }

            returnMessage.AccountId = apprenticeship.AccountId;
            returnMessage.Priority = apprenticeship.Priority;
            return returnMessage;
        }

        public async Task Reset()
        {
            paymentLogger.LogInfo($"Resetting actor for provider {Id}");
            await apprenticeships.ResetInitialiseFlag().ConfigureAwait(false);
        }

        protected override async Task OnActivateAsync()
        {
            await Initialise().ConfigureAwait(false);

            await base.OnActivateAsync().ConfigureAwait(false);
        }

        private async Task Initialise()
        {
            if (await apprenticeships.IsInitialiseFlagIsSet().ConfigureAwait(false)) return;

            paymentLogger.LogInfo($"Initialising actor for provider {Id}");

            var providerCommitments = await apprenticeshipRepository.ApprenticeshipsForProvider(long.Parse(Id.ToString())).ConfigureAwait(false);

            var groupedCommitments = providerCommitments.ToLookup(x => x.Uln);

            foreach (var group in groupedCommitments)
            {
                await this.apprenticeships.AddOrReplace(group.Key.ToString(), group.ToList()).ConfigureAwait(false);
            }

            paymentLogger.LogInfo($"Initialised actor for provider {Id}");

            await apprenticeships.SetInitialiseFlag().ConfigureAwait(false);
        }
    }
}
