using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.LevyFundedService
{
    [StatePersistence(StatePersistence.Persisted)]
    public class LevyFundedService : Actor, ILevyFundedService
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly ILifetimeScope lifetimeScope;
        private const string InitialisedKey = "Initialised";

        public LevyFundedService(
            ActorService actorService,
            ActorId actorId,
            IPaymentLogger paymentLogger,
            ILifetimeScope lifetimeScope) : base(actorService, actorId)
        {
            this.paymentLogger = paymentLogger;
            this.lifetimeScope = lifetimeScope;
        }

        public async Task Initialise()
        {
            if (await StateManager.ContainsStateAsync(InitialisedKey).ConfigureAwait(false)) return;

            paymentLogger.LogInfo($"Initialising actor for employer {Id}");
        }

        public async Task Reset()
        {
            paymentLogger.LogInfo($"Resetting actor for apprenticeship {Id}");
            await StateManager.TryRemoveStateAsync(InitialisedKey, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<ReadOnlyCollection<FundingSourcePaymentEvent>> HandleRequiredPayment(ApprenticeshipContractType1RequiredPaymentEvent message, CancellationToken none)
        {
            paymentLogger.LogVerbose($"Handling RequiredPayment for {Id}");

            await Initialise().ConfigureAwait(false);

            return null;
        }
    }
}
