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
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.DataLockService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class DataLockService : Actor, IDataLockService
    {
        private readonly IMapper mapper;
        private readonly IPaymentLogger paymentLogger;
        private readonly IDataCache<List<CommitmentModel>> commitments;
        private readonly ICommitmentRepository commitmentRepository;

        public DataLockService(
            ActorService actorService, 
            ActorId actorId, 
            IMapper mapper,
            IPaymentLogger paymentLogger, 
            ICommitmentRepository commitmentRepository, 
            IDataCache<List<CommitmentModel>> commitments) 
            : base(actorService, actorId)
        {
            this.mapper = mapper;
            this.paymentLogger = paymentLogger;
            this.commitmentRepository = commitmentRepository;
            this.commitments = commitments;
        }

        private const string InitialisedKey = "initialised";

        public async Task<DataLockEvent> HandleEarning(ApprenticeshipContractType1EarningEvent message, CancellationToken cancellationToken)
        {
            var commitmentsForUln = await commitments.TryGet(message.Learner.Uln.ToString(), cancellationToken)
                .ConfigureAwait(false);
            var commitment = commitmentsForUln.Value.FirstOrDefault();

            var returnMessage = mapper.Map<PayableEarningEvent>(message);
            returnMessage.AccountId = commitment.AccountId;
            returnMessage.Priority = commitment.Priority;
            return returnMessage;
        }

        public async Task Reset()
        {
            paymentLogger.LogInfo($"Resetting actor for provider {Id}");
            await StateManager.TryRemoveStateAsync(InitialisedKey, CancellationToken.None).ConfigureAwait(false);
            // TODO: When we can clear the list
            //await commitments.Clear().ConfigureAwait(false);
        }

        protected override async Task OnActivateAsync()
        {
            await Initialise().ConfigureAwait(false);

            await base.OnActivateAsync().ConfigureAwait(false);
        }

        private async Task Initialise()
        {
            if (await StateManager.ContainsStateAsync(InitialisedKey).ConfigureAwait(false)) return;

            paymentLogger.LogInfo($"Initialising actor for provider {Id}");

            var providerCommitments = await commitmentRepository.CommitmentsForProvider(long.Parse(Id.ToString())).ConfigureAwait(false);

            var groupedCommitments = providerCommitments.ToLookup(x => x.Uln);

            foreach (var group in groupedCommitments)
            {
                await this.commitments.AddOrReplace(group.Key.ToString(), group.ToList()).ConfigureAwait(false);
            }

            paymentLogger.LogInfo($"Initialised actor for provider {Id}");

            await StateManager.TryAddStateAsync(InitialisedKey, true).ConfigureAwait(false);
        }
    }
}
