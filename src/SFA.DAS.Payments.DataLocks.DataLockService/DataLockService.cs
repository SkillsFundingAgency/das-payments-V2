using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
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
        private readonly ActorService actorService;
        private readonly ActorId actorId;
        private readonly IPaymentLogger paymentLogger;
        private readonly IActorDataCache<List<ApprenticeshipModel>> apprenticeships;
        private readonly IDataLockProcessor dataLockProcessor;
        private readonly IApprenticeshipRepository apprenticeshipRepository;

        public DataLockService(
            ActorService actorService, 
            ActorId actorId,
            IPaymentLogger paymentLogger, 
            IApprenticeshipRepository apprenticeshipRepository,
            IActorDataCache<List<ApprenticeshipModel>> apprenticeships,
            IDataLockProcessor dataLockProcessor) 
            : base(actorService, actorId)
        {
            this.actorService = actorService;
            this.actorId = actorId;
            this.paymentLogger = paymentLogger;
            this.apprenticeshipRepository = apprenticeshipRepository;
            this.apprenticeships = apprenticeships;
            this.dataLockProcessor = dataLockProcessor;
        }

        public async Task<DataLockEvent> HandleEarning(ApprenticeshipContractType1EarningEvent message,
            CancellationToken cancellationToken)
        {
            return await dataLockProcessor.Validate(message, cancellationToken);
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

            var providerApprenticeships = await apprenticeshipRepository.ApprenticeshipsForProvider(long.Parse(Id.ToString())).ConfigureAwait(false);

            var groupedApprenticeships = providerApprenticeships.ToLookup(x => x.Uln);

            foreach (var group in groupedApprenticeships)
            {
                await this.apprenticeships.AddOrReplace(group.Key.ToString(), group.ToList()).ConfigureAwait(false);
            }

            paymentLogger.LogInfo($"Initialised actor for provider {Id}");

            await apprenticeships.SetInitialiseFlag().ConfigureAwait(false);
        }
    }
}
